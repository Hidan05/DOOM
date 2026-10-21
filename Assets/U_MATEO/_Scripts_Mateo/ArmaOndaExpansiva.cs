using UnityEngine;

public class ArmaOndaExpansiva : MonoBehaviour
{
    [Header("Wave Configuration")]
    public float explosionRadius = 6f, pushForce = 900f, pushElevation = 1f;
    public float fireRate = 0.5f, shootRange = 15f;
    public int damage = 50;

    [Header("References & Layers")]
    public LayerMask impactLayer;
    public AudioClip shootSound;

    // Calcula el punto de impacto mediante un Raycast y reproduce el sonido del disparo
    public void ExecuteExpansiveShot(Transform origin, AudioSource audioSource)
    {
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

        // Si el rayo impacta un objeto, toma ese punto; de lo contrario, toma el punto máximo del alcance
        Vector3 impactPoint = Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, shootRange, impactLayer) 
            ? hit.point 
            : origin.position + (origin.forward * shootRange);

        GenerateWaveAtPoint(impactPoint, origin.rotation);
    }

    // Procesa las colisiones dentro del radio de la onda
    private void GenerateWaveAtPoint(Vector3 center, Quaternion originRotation)
    {
        // Obtiene todos los colisionadores atrapados en la esfera de impacto
        Collider[] hitColliders = Physics.OverlapSphere(center, explosionRadius, impactLayer);

        foreach (Collider col in hitColliders)
        {
            // Aplica el daño y ejecuta la muerte si el objeto afectado tiene el componente Enemy
            Enemy enemy = col.GetComponentInParent<Enemy>();
            if (enemy) 
            {
                enemy.Damage(damage, originRotation);
                enemy.Die(); // <-- Ahora sí está correctamente dentro del ciclo
            }

            // Delega el efecto físico de empuje al sistema de IA del enemigo
            EnemyAI enemyAI = col.GetComponentInParent<EnemyAI>();
            if (enemyAI) enemyAI.ReceiveWavePush(center, pushForce, explosionRadius, pushElevation);
        }
    }

    // Dibuja una esfera de referencia en el editor de Unity para previsualizar el alcance de la explosión
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}