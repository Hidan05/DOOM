using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDLlaves : MonoBehaviour
{
    [Header("Icono de la llave")]
    public GameObject iconoLlave;

    void Start()
    {
        MostrarLlave(false);
    }

    public void MostrarLlave(bool tieneLlave)
    {
        if (iconoLlave != null)
        {
            iconoLlave.SetActive(tieneLlave);
        }
    }
}
