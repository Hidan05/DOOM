using UnityEngine;

public class HUDLlaves : MonoBehaviour
{
    public static HUDLlaves Instance;

    [Header("Key UI Icon")]
    public GameObject keyIcon;

    void Awake()
    {
        // Singleton para poder llamarlo fácilmente desde el KeyManager
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Oculta el icono de la llave al iniciar el juego
        ToggleKeyIcon(false);
    }

    // Muestra u oculta el icono de la llave en la interfaz
    public void ToggleKeyIcon(bool hasKey)
    {
        if (keyIcon) keyIcon.SetActive(hasKey);
    }
}