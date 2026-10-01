
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Guarda la salud maxima del jugador
    public int maxHealth;

    // Se inicializa la variable que se modifica en el juego
    [HideInInspector] public int health;

    // Start is called before the first frame update
    void Start()
    {
        // Se le asigna a la variable el valor de salud maxima
        health = maxHealth;
    }

    public void PlayerDamage()
    {
        if(health > 1)
        {
            health --;
            Debug.Log(health);
        }
        else if(health == 1)
        {
            Debug.Log("Personaje Muere");
        }
    }

}
