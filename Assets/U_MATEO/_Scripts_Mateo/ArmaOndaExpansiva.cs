using UnityEngine;

public class ArmaOndaExpansiva : MonoBehaviour
{
    [Header("Wave Configuration")]
    public float horizontalRange = 6f;  
    public float verticalRange = 2f;    
    public float pushForce = 900f, pushElevation = 1f;
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

    // Procesa las colisiones dentro de un área controlada (caja) nivelada al suelo
    private void GenerateWaveAtPoint(Vector3 center, Quaternion originRotation)
    {
        Vector3 halfExtents = new Vector3(horizontalRange, verticalRange, horizontalRange);
        
        // CORRECCIÓN: Obligamos a que la caja siempre esté plana respecto al suelo (eje Y), 
        // ignorando si el jugador estaba apuntando hacia el cielo o hacia el piso.
        Quaternion flatRotation = Quaternion.Euler(0, originRotation.eulerAngles.y, 0);

        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, flatRotation, impactLayer);

        foreach (Collider col in hitColliders)
        {
            // Aplica el daño y ejecuta la muerte si el objeto afectado tiene el componente Enemy
            Enemy enemy = col.GetComponentInParent<Enemy>();
            if (enemy) 
            {
                enemy.Damage(damage, originRotation);
                enemy.Die(); 
            }

            // Delega el efecto físico de empuje al sistema de IA del enemigo
            EnemyAI enemyAI = col.GetComponentInParent<EnemyAI>();
            if (enemyAI) enemyAI.ReceiveWavePush(center, pushForce, horizontalRange, pushElevation);
        }
    }

    // Dibuja una caja de referencia en el editor de Unity para previsualizar el alcance exacto
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // CORRECCIÓN VISUAL: Modificamos la matriz del Gizmo para que también dibuje la caja 
        // completamente nivelada al suelo en la ventana de Scene de Unity.
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(0, transform.eulerAngles.y, 0), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(horizontalRange * 2, verticalRange * 2, horizontalRange * 2));
    }
}