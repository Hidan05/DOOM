using UnityEngine;

public class ObjectsPickup : MonoBehaviour
{
    public bool isExpansiveWeapon = true;
    public Guns normalWeaponData;

    public float rotationSpeed = 60f, floatAmplitude = 0.25f, floatSpeed = 2f;
    public AudioClip pickupSound;
    public GameObject pickupVFX;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        transform.position = startPos + Vector3.up * (Mathf.Sin(Time.time * floatSpeed) * floatAmplitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || GunController.Instance == null) return;

        // CORRECCIÓN: Ahora añade el arma al inventario en lugar de solo equiparla temporalmente
        if (isExpansiveWeapon) GunController.Instance.AddExpansiveWeapon();
        else if (normalWeaponData) GunController.Instance.AddNormalWeapon(normalWeaponData);

        if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        if (pickupVFX) Instantiate(pickupVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}