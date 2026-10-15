using UnityEngine;

public class ArmaOndaExpansiva : MonoBehaviour
{
    [Header("Configuración de la Onda")]
    public float radioExplosion = 6f;      // Alcance de la onda al chocar
    public float fuerzaEmpuje = 900f;     // Fuerza con la que empuja
    public float elevacionEmpuje = 1f;    // Levanta a los enemigos del suelo
    public int daño = 50;                 // Daño que causa la explosión
    public float cadenciaDisparo = 0.5f;   // Tiempo entre disparos
    public float alcanceDisparo = 15f;    // Distancia máxima del disparo

    [Header("Referencias y Capas")]
    public LayerMask capaImpacto;        // Paredes, suelo y enemigos
    public AudioClip sonidoDisparo;
    public GameObject vfxOndaImpacto;      // Prefab de partículas en el choque

    public void EjecutarDisparoOnda(Transform origen, AudioSource audioSource)
    {
        if (audioSource != null && sonidoDisparo != null)
        {
            audioSource.PlayOneShot(sonidoDisparo);
        }

        RaycastHit hit;
        Vector3 puntoDeImpacto;

        if (Physics.Raycast(origen.position, origen.forward, out hit, alcanceDisparo, capaImpacto))
        {
            puntoDeImpacto = hit.point;
        }
        else
        {
            puntoDeImpacto = origen.position + (origen.forward * alcanceDisparo);
        }

        GenerarOndaEnPunto(puntoDeImpacto, origen.rotation);
    }

    private void GenerarOndaEnPunto(Vector3 centro, Quaternion rotacionOrigen)
    {
        if (vfxOndaImpacto != null)
        {
            Instantiate(vfxOndaImpacto, centro, Quaternion.identity);
        }

        Collider[] objetosAlcanzados = Physics.OverlapSphere(centro, radioExplosion, capaImpacto);

        foreach (Collider col in objetosAlcanzados)
        {
            // Aplica daño
            Enemy enemy = col.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage(daño, rotacionOrigen);
            }

            // Gestiona el empuje desactivando temporalmente la IA
            EnemyAI enemyAI = col.GetComponentInParent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.RecibirEmpujeOnda(centro, fuerzaEmpuje, radioExplosion, elevacionEmpuje);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}