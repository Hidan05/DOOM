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

    // Método para marcar que la llave ha sido obtenida
    public void CollectKey()
    {
        hasKey = true;
        Debug.Log("¡Llave recolectada!");
    }
}