using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
     public PlayerStats jugador;
    public PlayerArmor armadura;

    public void RecibirGolpe()
    {
        if (armadura != null && armadura.AbsorberGolpe())
        {
            Debug.Log("¡La armadura absorbió el golpe!");
            return;
        }

        if (jugador != null)
        {
            jugador.PlayerDamage();
        }
    }
}
