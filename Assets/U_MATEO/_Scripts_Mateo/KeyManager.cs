using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;
    public bool hasKey = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CollectKey()
    {
        hasKey = true;

        // Activa la imagen de la llave en el HUD cuando la recoge
        if (HUDLlaves.Instance != null)
        {
            HUDLlaves.Instance.ToggleKeyIcon(true);
        }

        Debug.Log("¡Llave recolectada!");
    }
}