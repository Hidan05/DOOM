using UnityEngine;

public class Trofeo : MonoBehaviour
{
    public GameObject GameOver;

    void Start()
    {
        GameOver.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mostrar menú de Game Over
            GameOver.SetActive(true);

            // Ocultar el trofeo
            gameObject.SetActive(false);

            // Pausar el juego
            Time.timeScale = 0f;
        }
    }
}
