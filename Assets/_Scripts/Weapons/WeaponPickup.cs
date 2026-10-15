using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Tipo de Arma")]
    public bool esArmaExpansiva = true;   // Si es true, activa el modo onda. Si es false, equipa arma normal.

    [Header("Datos de Arma Normal")]
    public Guns armaNormalData;           // Solo necesario si 'esArmaExpansiva' es false

    [Header("Animación de Ítem")]
    public float velocidadRotacion = 60f; 
    public float amplitudFlotacion = 0.25f; 
    public float velocidadFlotacion = 2f;  

    [Header("Efectos Opcionales")]
    public AudioClip sonidoRecogida;      
    public GameObject efectoVFX;          

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Animación de rotación y flotación en el mapa
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlotacion) * amplitudFlotacion;
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RecogerArma();
        }
    }

    private void RecogerArma()
    {
        if (GunController.Instance == null)
        {
            Debug.LogWarning("No se encontró GunController.Instance en la escena.");
            return;
        }

        if (esArmaExpansiva)
        {
            // Equipar arma expansiva (no requiere ScriptableObject)
            GunController.Instance.EquiparArmaExpansiva();
        }
        else
        {
            // Equipar arma normal enviando su ScriptableObject
            if (armaNormalData != null)
            {
                GunController.Instance.EquiparArmaNormal(armaNormalData);
            }
            else
            {
                Debug.LogError("No asignaste el ScriptableObject 'armaNormalData' en este Pickup.");
                return;
            }
        }

        // Efectos de sonido y partículas al recoger
        if (sonidoRecogida != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);
        }

        if (efectoVFX != null)
        {
            Instantiate(efectoVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}