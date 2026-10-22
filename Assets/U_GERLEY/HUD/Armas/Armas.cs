
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Armas : MonoBehaviour
{
    [Header("Iconos de las armas")]
    public Image Arma1;
    public Image Arma2;

    [Header("Munición")]
    public TextMeshProUGUI textoMunicion;

    public void MostrarMunicion(int actual, int reserva)
    {
        if (textoMunicion != null)
            textoMunicion.text = actual + " / " + reserva;
    }

    public void MostrarArmaSeleccionada(int numeroArma)
    {
        if (Arma1 != null)
            Arma1.enabled = numeroArma == 1;

        if (Arma2 != null)
            Arma2.enabled = numeroArma == 2;
    }
}

