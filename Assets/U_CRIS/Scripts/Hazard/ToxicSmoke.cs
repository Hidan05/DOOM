using System.Collections;
using UnityEngine;

public class ToxicSmoke : MonoBehaviour
{
    [Header("Configuración del Humo Tóxico")]
    [Range(3f, 4f)]
    public float intervaloDaño = 3.5f; // 3.5 segundos

    [Range(0.01f, 1f)]
    public float porcentajeDanio = 0.15f; // 15% de daño

    [Header("Configuración de Visibilidad (Niebla)")]
    public bool afectarVisibilidad = true;
    public Color colorHumo = new Color(0.2f, 0.5f, 0.2f); // Tono verdoso tóxico
    public float densidadNieblaAdentro = 0.15f; // Qué tan espeso es el humo (mayor número = menos vista)

    private float densidadNieblaOriginal;
    private bool fogOriginal;
    private Color colorNieblaOriginal;

    private Coroutine corrutinaDaño;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats statsJugador = other.GetComponent<PlayerStats>();
        if (statsJugador != null)
        {
            Debug.Log("¡El jugador ha entrado en la zona de humo tóxico!");

            // Activamos el efecto visual de humo denso en la cámara/escena
            if (afectarVisibilidad)
            {
                ActivarEfectoHumoVisual(true);
            }

            if (corrutinaDaño == null)
            {
                corrutinaDaño = StartCoroutine(AplicarDañoPeriodico(statsJugador));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerStats statsJugador = other.GetComponent<PlayerStats>();
        if (statsJugador != null)
        {
            Debug.Log("¡El jugador ha salido del humo tóxico a salvo!");

            // Desactivamos el efecto visual de humo
            if (afectarVisibilidad)
            {
                ActivarEfectoHumoVisual(false);
            }

            if (corrutinaDaño != null)
            {
                StopCoroutine(corrutinaDaño);
                corrutinaDaño = null;
            }
        }
    }

    void ActivarEfectoHumoVisual(bool entrar)
    {
        if (entrar)
        {
            // Guardamos los valores actuales de la niebla para restaurarlos luego
            fogOriginal = RenderSettings.fog;
            densidadNieblaOriginal = RenderSettings.fogDensity;
            colorNieblaOriginal = RenderSettings.fogColor;

            // Aplicamos la densidad de humo tóxico
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = densidadNieblaAdentro;
            RenderSettings.fogColor = colorHumo;
        }
        else
        {
            // Restauramos los valores originales del entorno
            RenderSettings.fog = fogOriginal;
            RenderSettings.fogDensity = densidadNieblaOriginal;
            RenderSettings.fogColor = colorNieblaOriginal;
        }
    }

    IEnumerator AplicarDañoPeriodico(PlayerStats stats)
    {
        while (stats.health > 0)
        {
            yield return new WaitForSeconds(intervaloDaño);

            int dañoAEjecutar = Mathf.Max(1, Mathf.RoundToInt(stats.maxHealth * porcentajeDanio));
            stats.health -= dañoAEjecutar;

            if (stats.health < 0) stats.health = 0;

            Debug.Log("¡HUMO TÓXICO! Daño recibido: -" + dañoAEjecutar + " HP. Salud restante: " + stats.health + "/" + stats.maxHealth);

            if (stats.health <= 0)
            {
                Debug.Log("¡Personaje Muere por inhalación de humo tóxico!");
                // Aseguramos quitar la niebla si muere dentro del gas
                if (afectarVisibilidad) ActivarEfectoHumoVisual(false);
                break;
            }
        }

        corrutinaDaño = null;
    }
}