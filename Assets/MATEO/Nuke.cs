using UnityEngine;
using System.Collections;

public class Nuke : MonoBehaviour
{
    public Transform manoSecundaria;
    
    [Header("Temblor de Cámara (Inclinación)")]
    public Camera camaraPrincipal; 
    public float duracionTemblor = 0.4f;
    [Tooltip("Grados de inclinación (Ej: 5 o 10)")]
    public float magnitudTemblor = 5f; // IMPORTANTE: Ahora son grados, pon un número mayor como 5 o 10

    [Header("Animación de Espera")]
    public float velocidadRotacion = 60f;
    public float amplitudFlotacion = 0.25f;
    public float velocidadFlotacion = 2f;

    private bool equipado = false;
    private float posicionInicialY;

    void Start()
    {
        posicionInicialY = transform.position.y;
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
    }

    void Update()
    {
        // 1. Flotar y rotar
        if (!equipado)
        {
            transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);
            float nuevaY = posicionInicialY + Mathf.Sin(Time.time * velocidadFlotacion) * amplitudFlotacion;
            transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
        }

        // 3 y 4: Activar
        if (equipado && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(TemblorYDestruccion());
        }
    }

    // 2. Recoger al pasar (con tu posición exacta)
    void OnTriggerEnter(Collider other) 
    { 
        if (!equipado && other.CompareTag("Player")) 
        {
            equipado = true;
            transform.SetParent(manoSecundaria);
            transform.localPosition = new Vector3(0.89f, -0.34f, 0.98f);
            transform.localRotation = Quaternion.Euler(0f, 50f, 0f);
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    IEnumerator TemblorYDestruccion()
    {
        if (camaraPrincipal != null)
        {
            Quaternion rotOriginal = camaraPrincipal.transform.localRotation;
            float tiempo = 0f;

            while (tiempo < duracionTemblor)
            {
                // Multiplicar el tiempo por 50 hace que vaya de izquierda a derecha muy rápido
                float inclinacionZ = Mathf.Sin(tiempo * 50f) * magnitudTemblor; 
                
                // Aplicamos la inclinación en el eje Z (Roll)
                camaraPrincipal.transform.localRotation = rotOriginal * Quaternion.Euler(0, 0, inclinacionZ);
                
                tiempo += Time.deltaTime;
                yield return null; 
            }
            // Restaurar la rotación normal
            camaraPrincipal.transform.localRotation = rotOriginal;
        }

        // Destruir enemigos
        foreach (GameObject enemigo in GameObject.FindGameObjectsWithTag("Enemy")) Destroy(enemigo);

        // Destruir el activador
        Destroy(gameObject);
    }
}