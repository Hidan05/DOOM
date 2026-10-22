using UnityEngine;

public class ObjectsPickup : MonoBehaviour
{
    public bool isExpansiveWeapon = true;
    public Guns normalWeaponData;

    public float rotationSpeed = 60f, floatAmplitude = 0.25f, floatSpeed = 2f;
    public AudioClip pickupSound;
    public GameObject pickupVFX;

    private Vector3 startPos;

    // Guarda la posición inicial para que el ítem flote en su lugar
    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rota el ítem sobre su propio eje Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        
        // Calcula y aplica el movimiento de flotación arriba y abajo
        transform.position = startPos + Vector3.up * (Mathf.Sin(Time.time * floatSpeed) * floatAmplitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que choca no es el jugador, o si no existe el controlador de armas, no hace nada
        if (!other.CompareTag("Player") || GunController.Instance == null) return;

        // Equipa el arma expansiva o la normal dependiendo de la configuración
        if (isExpansiveWeapon) GunController.Instance.EquipExpansiveWeapon();
        else if (normalWeaponData) GunController.Instance.EquipNormalWeapon(normalWeaponData);

        // Reproduce los efectos de sonido y partículas si fueron asignados en el inspector
        if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        if (pickupVFX) Instantiate(pickupVFX, transform.position, Quaternion.identity);

        // Destruye el ítem del mapa una vez recogido
        Destroy(gameObject);
    }
}