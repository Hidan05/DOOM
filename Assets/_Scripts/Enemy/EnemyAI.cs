using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 25f;  // Distancia máxima para buscar puntos de patrullaje
    public int numberOfPoints = 3;    // Cantidad de puntos a generar automáticamente
    
    // Almacena las coordenadas aleatorias generadas
    private Vector3[] patrolPoints;
    private int currentPointIndex = 0;

    // Referencias
    private EnemyAggro enemyAggro;
    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;

    // Control de físicas
    private bool isKnockedBack = false;

    [Header("Speeds")]
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 6.5f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        enemyAggro = GetComponent<EnemyAggro>();

        if (playerTransform == null)
        {
            PlayerMovement playerObj = FindAnyObjectByType<PlayerMovement>();
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        // Genera los puntos aleatorios de patrullaje al nacer
        GeneratePatrolPoints();
        NextPoint();
    }

    // Calcula puntos al azar sobre el NavMesh basados en su posición inicial
    private void GeneratePatrolPoints()
    {
        patrolPoints = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            // Genera una dirección aleatoria dentro de una esfera
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position; // Se centra en donde spawneó el enemigo

            NavMeshHit hit;
            // Busca el punto transitable más cercano en el NavMesh (radio de búsqueda = patrolRadius)
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolPoints[i] = hit.position;
            }
            else
            {
                // Si falla (muy raro), asigna su posición actual como respaldo
                patrolPoints[i] = transform.position; 
            }
        }
    }

    private void NextPoint()
    {
        // Verifica que el agente y el arreglo sean válidos
        if (patrolPoints == null || patrolPoints.Length == 0 || navMeshAgent == null || !navMeshAgent.enabled || !navMeshAgent.isOnNavMesh)
            return;

        // Asigna la coordenada actual como destino
        navMeshAgent.destination = patrolPoints[currentPointIndex];
        
        // Pasa al siguiente índice y vuelve a 0 si llega al final
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    private void Update()
    {
        if (isKnockedBack) return;

        EnemyMovement();
    }

    private void EnemyMovement()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled || !navMeshAgent.isOnNavMesh) return;

        if (enemyAggro != null && enemyAggro.isAggro && playerTransform != null)
        {
            navMeshAgent.SetDestination(playerTransform.position);
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.stoppingDistance = 3;
        }
        else
        {
            navMeshAgent.stoppingDistance = 0;
            navMeshAgent.speed = patrolSpeed;

            // Si está patrullando y llegó a su destino, pasa al siguiente punto
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                NextPoint();
            }
        }
    }

    public void ReceiveWavePush(Vector3 explosionCenter, float force, float radius, float elevation)
    {
        StartCoroutine(KnockbackRoutine(explosionCenter, force, radius, elevation));
    }

    private IEnumerator KnockbackRoutine(Vector3 explosionCenter, float force, float radius, float elevation)
    {
        isKnockedBack = true;

        if (navMeshAgent != null) navMeshAgent.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;

            Vector3 adjustedCenter = explosionCenter;
            if (adjustedCenter.y >= transform.position.y)
            {
                adjustedCenter.y = transform.position.y - 0.5f; 
            }

            rb.AddExplosionForce(force, adjustedCenter, radius, elevation, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(0.5f);

        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }

        NavMeshHit hit;
        // Se mantiene el radio de reconexión corto que corregimos en el mensaje anterior (2.0f)
        if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;

            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.Warp(hit.position);
            }
        }
        else
        {
            if (navMeshAgent != null) navMeshAgent.enabled = true;
        }

        isKnockedBack = false;

        if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            if (enemyAggro == null || !enemyAggro.isAggro)
            {
                NextPoint();
            }
        }
    }
    
    // Dibuja la zona de patrullaje en el editor de Unity (línea amarilla)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}