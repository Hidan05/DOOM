using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaduraPickup : MonoBehaviour
{
     public PlayerArmor armaduraJugador;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (armaduraJugador != null)
            {
                armaduraJugador.RecogerArmadura();
                Destroy(gameObject);
            }
        }
    }
}
