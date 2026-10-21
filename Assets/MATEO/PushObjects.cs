using UnityEngine;

public class PushObjects : MonoBehaviour
{
    [Header("Fuerza de empuje")]
    public float pushForce = 2.0f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        // Verificamos que el objeto golpeado tenga un Rigidbody y se pueda mover
        if (rb != null && !rb.isKinematic)
        {
            // Calculamos la dirección del empuje ignorando el eje Y (para no empujar hacia abajo/arriba)
            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            // Aplicamos la velocidad al cubo en esa dirección
            rb.velocity = pushDirection * pushForce;
        }
    }
}