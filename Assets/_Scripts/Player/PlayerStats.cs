using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Guarda la salud máxima del jugador
    public int maxHealth;

    // Referencia a la barra de vida
    public BarraDeVida barraDevida;

    // Salud actual del jugador
    [HideInInspector] public int health;

    void Start()
    {
        // Asigna la salud máxima al comenzar
        health = maxHealth;

        // Inicializa la barra si está asignada
        if (barraDevida != null)
        {
            barraDevida.InicializarBarraDeVida(health);
        }
        else
        {
            Debug.LogWarning
            (
                "Debes asignar la BarraDeVida en el Inspector."
            );
        }
    }

    public void PlayerDamage()
    {
        if (health > 1)
        {
            health--;

            // Actualiza la barra después de recibir daño
            if (barraDevida != null)
            {
                barraDevida.CambiarVidaActual(health);
            }

            Debug.Log(health);
        }
        else if (health == 1)
        {
            Debug.Log("Personaje Muere");
        }
    }
}

