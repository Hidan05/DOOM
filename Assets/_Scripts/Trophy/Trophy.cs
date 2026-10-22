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
        // 1. Confirma si algo está tocando el trofeo
        Debug.Log("Objeto detectado en el trofeo: " + other.name + " | Tag: " + other.tag);

        // Si no tiene el Tag "Player", no hace nada
        if (!other.CompareTag("Player")) return;

        // 2. Comprueba si existe el KeyManager en la escena
        if (KeyManager.Instance == null)
        {
            Debug.LogError("ERROR: No existe el objeto KeyManager en la escena.");
            return;
        }

        // 3. Comprueba si el jugador tiene la llave
        if (!KeyManager.Instance.hasKey)
        {
            Debug.Log("Tocaste el trofeo, pero NO tienes la llave aún.");
            return;
        }

        // 4. Victoria
        Debug.Log("¡Trofeo recogido con éxito!");

        if (gameOverUI) gameOverUI.SetActive(true);
        gameObject.SetActive(false);
        Time.timeScale = 0f;
    }
}