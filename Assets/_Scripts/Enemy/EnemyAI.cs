using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Almacena los puntos de movimiento de los enemigos.
    public Transform[] points;

    // Define el primer destino como el primer elemento de la lista.
    private int destPoint = 0;

    // Variable para almacenar el componente del Script de Enemy Aggro
    private EnemyAggro enemyAggro;

    // Para almacenar el transform del jugador.
    private Transform playerTransform;

    // Se almacena la variable para el componente del Nav Mesh.
    private NavMeshAgent navMeshAgent;

    // Referencia al Rigidbody para las físicas de empuje
    private Rigidbody rb;

    // Controla si el enemigo está siendo empujado por la onda
    private bool isKnockedBack = false;

    // Variable que va a guardar las velocidades de persecución y de patrulla
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

        NextPoint();
    }

    private void NextPoint()
    {
        // Verifica que el agente esté activo y sobre el NavMesh antes de asignar destino
        if (points == null || points.Length == 0 || navMeshAgent == null || !navMeshAgent.enabled || !navMeshAgent.isOnNavMesh)
            return;

        navMeshAgent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }

    private void Update()
    {
        if (isKnockedBack) return;

        EnemyMovement();
    }

    private void EnemyMovement()
    {
        // Evita ejecutar lógica de movimiento si el NavMeshAgent no está posicionado correctamente
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

            // Se valida que esté en el NavMesh ANTES de consultar remainingDistance
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

        // 1. Apagar la IA
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        // 2. Aplicar físicas ajustando la altura del impacto
        if (rb != null)
        {
            rb.isKinematic = false;

            // --- CORRECCIÓN DE ALTURA ---
            // Forzamos a que el centro de la explosión esté al nivel del piso o por debajo de la cápsula
            Vector3 adjustedCenter = explosionCenter;
            if (adjustedCenter.y >= transform.position.y)
            {
                adjustedCenter.y = transform.position.y - 0.5f; 
            }

            rb.AddExplosionForce(force, adjustedCenter, radius, elevation, ForceMode.Impulse);
        }

        // 3. Tiempo de vuelo
        yield return new WaitForSeconds(0.5f);

        // 4. Detener movimiento
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }

        // 5. Reubicar en el NavMesh de forma segura
        NavMeshHit hit;
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
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
            }
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
}