using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Configuration")]
    public GameObject enemyPrefab;    // El prefab de tu enemigo
    public Transform[] spawnPoints;   // Los puntos donde aparecerán
    public float spawnInterval = 3f;  // Tiempo en segundos entre cada aparición
    public int maxEnemies = 10;       // Límite máximo de enemigos en el mapa al mismo tiempo

    void Start()
    {
        // Inicia la rutina de generación de enemigos
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // Bucle infinito que se ejecuta mientras el objeto Spawner exista
        while (true)
        {
            // Cuenta cuántos enemigos hay actualmente vivos usando su etiqueta
            int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

            // Si hay espacio disponible, hay puntos de aparición configurados y un prefab asignado, spawnea uno
            if (currentEnemies < maxEnemies && spawnPoints.Length > 0 && enemyPrefab)
            {
                SpawnEnemy();
            }

            // Espera el tiempo definido antes de volver a intentar generar otro
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        // Elige un índice al azar dentro del arreglo de puntos
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // Crea el enemigo en la posición y rotación del punto elegido
        Instantiate(enemyPrefab, selectedPoint.position, selectedPoint.rotation);
    }
}