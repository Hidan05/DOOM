using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && KeyManager.Instance != null)
        {
            KeyManager.Instance.CollectKey();
            Destroy(gameObject); // Elimina el ítem de la llave del mapa al recogerlo
        }
    }
}