using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public enum TipoProyectil { Pistola, Escopeta }

    [Header("Configuración de Munición")]
    public TipoProyectil tipoDeMunicion;
    public int cantidadBalas = 15;

    [Header("Materiales")]
    public Material materialPistola;
    public Material materialEscopeta;

    [Header("Efecto Flotante")]
    public float velocidadRotacion = 50f;

    private MeshRenderer meshRenderer;

    void Start()
    {
        // Asegurar que tenga Trigger activo
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        meshRenderer = GetComponent<MeshRenderer>();
        AplicarColor();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime);
    }

    void AplicarColor()
    {
        if (meshRenderer == null) return;

        if (tipoDeMunicion == TipoProyectil.Pistola && materialPistola != null)
        {
            meshRenderer.material = materialPistola;
        }
        else if (tipoDeMunicion == TipoProyectil.Escopeta && materialEscopeta != null)
        {
            meshRenderer.material = materialEscopeta;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<GunController>() != null || other.GetComponentInParent<GunController>() != null)
        {
            if (AmmoManager.Instance != null)
            {
                // Unificamos el texto en español para que coincida perfectamente con el AmmoManager y GunController
                string ammoName = (tipoDeMunicion == TipoProyectil.Pistola) ? "Pistola" : "Escopeta";

                // Enviamos ambos parámetros que requiere el AddAmmo(string, int)
                AmmoManager.Instance.AddAmmo(ammoName, cantidadBalas);
                Debug.Log("¡Recogiste " + cantidadBalas + " balas de " + tipoDeMunicion + "!");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("¡AmmoManager no está en la escena!");
            }
        }
    }
}