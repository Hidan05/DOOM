using UnityEngine;
using System.Collections;

public class Nuke : MonoBehaviour
{
    public Transform secondaryHand;
    
    [Header("Camera Shake (Tilt)")]
    public Camera mainCamera; 
    public float shakeDuration = 0.4f;
    [Tooltip("Grados de inclinación (Ej: 5 o 10)")]
    public float shakeMagnitude = 5f; 

    [Header("Idle Animation")]
    public float rotationSpeed = 60f;
    public float floatAmplitude = 0.25f;
    public float floatSpeed = 2f;

    private bool isEquipped = false;
    private float initialPositionY;

    void Start()
    {
        // Guarda la altura inicial para que el cálculo de flotación sea exacto
        initialPositionY = transform.position.y;
        
        // Busca y asigna la cámara principal automáticamente si olvidaste ponerla en el Inspector
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. Animación de flotar y rotar en el mapa (solo si nadie lo ha recogido)
        if (!isEquipped)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
            float newY = initialPositionY + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        // 2. Activar la nuke si ya la tienes en la mano y presionas la tecla F
        if (isEquipped && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ShakeAndDestroy());
        }
    }

    // 3. Detectar cuando el jugador pasa sobre el ítem para recogerlo
    void OnTriggerEnter(Collider other) 
    { 
        if (!isEquipped && other.CompareTag("Player")) 
        {
            isEquipped = true;
            
            // Emparenta la Nuke a la mano secundaria y le asigna la posición y rotación exactas
            transform.SetParent(secondaryHand);
            transform.localPosition = new Vector3(0.89f, -0.34f, 0.98f);
            transform.localRotation = Quaternion.Euler(0f, 50f, 0f);
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    // Corrutina encargada de agitar la pantalla y eliminar a los enemigos
    IEnumerator ShakeAndDestroy()
    {
        if (mainCamera != null)
        {
            // Guarda la rotación original de la cámara para no desconfigurarla luego
            Quaternion originalRot = mainCamera.transform.localRotation;
            float elapsedTime = 0f;

            // Bucle que ejecuta el temblor mientras no se supere la duración límite
            while (elapsedTime < shakeDuration)
            {
                // Multiplicar el tiempo por 50 hace que la oscilación (izquierda-derecha) sea muy rápida
                float tiltZ = Mathf.Sin(elapsedTime * 50f) * shakeMagnitude; 
                
                // Aplica la inclinación calculada en el eje Z (Roll)
                mainCamera.transform.localRotation = originalRot * Quaternion.Euler(0, 0, tiltZ);
                
                elapsedTime += Time.deltaTime;
                yield return null; 
            }
            
            // Restaura la rotación normal de la cámara al finalizar el temblor
            mainCamera.transform.localRotation = originalRot;
        }

        // Busca y destruye a todos los objetos en la escena que tengan el Tag "Enemigo"
        // Nota: Asegúrate de que tus enemigos tengan la etiqueta exactamente como "Enemigo"
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemigo")) 
        {
            Destroy(enemy);
        }

        // Destruye el objeto de la Nuke (la que tienes en la mano) después de haberla usado
        Destroy(gameObject);
    }
}