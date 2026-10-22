
using UnityEngine;

public class PlayerArmor : MonoBehaviour
{
    [Header("Configuración de armadura")]
    public int maxArmor = 3;
    public int armor = 0;

    [Header("HUD")]
    public GameObject barraArmaduraObjeto;
    public BarraDeVida barraArmadura;

    void Start()
    {
        armor = 0;

        if (barraArmadura != null)
        {
            barraArmadura.InicializarBarraDeVida(maxArmor);
            barraArmadura.CambiarVidaActual(0);
        }

        if (barraArmaduraObjeto != null)
            barraArmaduraObjeto.SetActive(false);
    }

    public void RecogerArmadura()
    {
        armor = maxArmor;

        if (barraArmaduraObjeto != null)
            barraArmaduraObjeto.SetActive(true);

        ActualizarBarra();

        Debug.Log("¡Armadura recogida!");
    }

    public bool AbsorberGolpe()
    {
        if (armor > 0)
        {
            armor--;
            ActualizarBarra();

            if (armor <= 0 && barraArmaduraObjeto != null)
                barraArmaduraObjeto.SetActive(false);

            Debug.Log("Armadura restante: " + armor);
            return true;
        }

        return false;
    }

    private void ActualizarBarra()
    {
        if (barraArmadura != null)
            barraArmadura.CambiarVidaActual(armor);
    }
}
