
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

    // Variable que va a guardar las velocidades de persecuci�n y de patrulla
    public float patrolSpeed;
    public float chaseSpeed;

    private void Awake()
    {
        // Se guarda el componente del Navmesh en la variable
        navMeshAgent = GetComponent<NavMeshAgent>();

    }
    private void Start()
    {
        // Se llenan las variables con los componentes
        enemyAggro = GetComponent<EnemyAggro>();

        // Se asegura que se guarda una referencia al jugador

        if (playerTransform == null)
            playerTransform = FindAnyObjectByType<PlayerMovement>().transform;


        
        // Se cambia el punto de destino para que el enemigo se mueva
        NextPoint();
    }

    private void NextPoint()
    {
        // Devuelve si no hay puntos de posici�n en el arreglo.
        if (points == null || points.Length == 0)
            return;

        // Se le da destino al enemigo, el cual es el primer punto de la lista (por ser destpoint = 0).
        navMeshAgent.destination = points[destPoint].position;

        // Se elige el siguiente punto en la lista. Todo numero menor que el divisor esel mismo m�dulo.
        destPoint = (destPoint + 1) % points.Length;
    }

    private void Update()
    {
        // Se llama al metodo de movimiento constantemente
       EnemyMovement();
    }

    private void EnemyMovement()
    {
        // Se valida si la variable de EnemyAggro es verdadera, para que el enemigo se mueva a la posici�n del jugador
        // Si es falsa el enemigo mantiene su posici�n
        if (enemyAggro.isAggro)
        {
            navMeshAgent.SetDestination(playerTransform.position);
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.stoppingDistance = 3;
        }
        else
        {
            navMeshAgent.stoppingDistance = 0;
            navMeshAgent.speed = patrolSpeed;

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                NextPoint();
            }        
        }
    }

}
