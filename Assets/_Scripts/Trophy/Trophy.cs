using UnityEngine;

public class Trophy : MonoBehaviour
{
    public GameObject gameOverUI;

    void Start()
    {
        if (gameOverUI) gameOverUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Verifica si existe el administrador de llaves y si el jugador ya la tiene
        if (KeyManager.Instance != null && !KeyManager.Instance.hasKey)
        {
            Debug.Log("¡Necesitas conseguir la llave antes de recoger el trofeo!");
            return; // Cancela la ejecución si no tiene la llave
        }

        // Muestra la pantalla de fin de juego / victoria
        if (gameOverUI) gameOverUI.SetActive(true);

        // Oculta el trofeo del mapa
        gameObject.SetActive(false);

        // Pausa el juego
        Time.timeScale = 0f;
    }
}