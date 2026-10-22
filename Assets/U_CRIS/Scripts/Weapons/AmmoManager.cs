using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance;

    [System.Serializable]
    public class AmmoData
    {
        public string ammoType;       // "Pistol" o "Shotgun"
        public int totalAmmo;         // Cantidad actual en reserva
    }

    public List<AmmoData> ammoList = new List<AmmoData>();

    private Dictionary<string, int> ammoDictionary = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Inicializar el diccionario con los datos del inspector
        foreach (var ammo in ammoList)
        {
            if (!ammoDictionary.ContainsKey(ammo.ammoType))
            {
                ammoDictionary.Add(ammo.ammoType, ammo.totalAmmo);
            }
        }
    }

    // Consultar cuánta munición hay de un tipo
    public int GetAmmo(string ammoType)
    {
        if (ammoDictionary.ContainsKey(ammoType))
        {
            return ammoDictionary[ammoType];
        }
        return 0;
    }

    // Añadir munición (cuando recoges un cartucho del mapa)
    public void AddAmmo(string ammoType, int amount)
    {
        if (ammoDictionary.ContainsKey(ammoType))
        {
            ammoDictionary[ammoType] += amount;
            Debug.Log("Munición recogida: " + ammoType + " | Total reserva: " + ammoDictionary[ammoType]);
        }
        else
        {
            ammoDictionary.Add(ammoType, amount);
        }
    }

    // Consumir munición al recargar
    public int ConsumeAmmo(string ammoType, int amountRequested)
    {
        if (!ammoDictionary.ContainsKey(ammoType)) return 0;

        int available = ammoDictionary[ammoType];
        int amountToGive = Mathf.Min(available, amountRequested);

        ammoDictionary[ammoType] -= amountToGive;
        Debug.Log("Munición restante en reserva (" + ammoType + "): " + ammoDictionary[ammoType]);
        
        return amountToGive;
    }
}