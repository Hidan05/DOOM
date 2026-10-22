using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Modelos 3D de las Armas")]
    public GameObject modeloPistola;
    public GameObject modeloEscopeta;

    public static GunController Instance;

    private BoxCollider gunTrigger;

    public Guns weapon;
    public LayerMask raycastLayerMask;

    private bool canFire;
    private bool isReloading;

    public AudioSource audioSource;
    [HideInInspector] public int currentAmmo;

    private int currentAmmoPistola = -1;
    private int currentAmmoEscopeta = -1;

    public Guns pistolaData;
    public Guns escopetaData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        gunTrigger = GetComponent<BoxCollider>();
    }

    void Start()
    {
        canFire = true;
        isReloading = false;

        if (pistolaData == null) pistolaData = Resources.Load<Guns>("Pistola_SO");
        if (escopetaData == null) escopetaData = Resources.Load<Guns>("Escopeta_SO");

        if (weapon == null && pistolaData != null)
        {
            weapon = pistolaData;
        }

        if (weapon != null)
        {
            currentAmmoPistola = pistolaData.magazineSize;
            currentAmmoEscopeta = escopetaData.magazineSize;
            currentAmmo = weapon == pistolaData ? currentAmmoPistola : currentAmmoEscopeta;
        }

        SetTrigger();
        EquiparArmaNormal(weapon);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isReloading)
        {
            CambiarArma();
        }
    }

    public void SetTrigger()
    {
        if (weapon == null) return;
        gunTrigger.size = new Vector3(weapon.horizontalRange, weapon.verticalRange, weapon.range);
        gunTrigger.center = new Vector3(0, (0.5f * weapon.verticalRange - 1f), weapon.range * 0.5f);
    }

    IEnumerator TimeToFire()
    {
        canFire = false;
        yield return new WaitForSeconds(weapon.fireRate);
        canFire = true;
    }

    public void Fire()
    {
        if (!canFire || isReloading) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("¡Sin munición en el cargador! Presiona R para recargar.");
            if (audioSource != null && weapon.emptySound != null)
            {
                audioSource.PlayOneShot(weapon.emptySound);
            }
            return;
        }

        currentAmmo--;
        GuardarBalasArmaActual();
        Debug.Log("Balas restantes en cargador: " + currentAmmo);

        if (audioSource != null && weapon.sound != null)
        {
            audioSource.PlayOneShot(weapon.sound);
        }

        foreach (Enemy enemy in EnemyManager.Instance.enemiesInRange)
        {
            RaycastHit hit;
            var direction = (enemy.transform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, direction, out hit, weapon.range * 1.5f, raycastLayerMask))
            {
                if (hit.transform == enemy.transform)
                {
                    Quaternion rot = Quaternion.LookRotation(-hit.normal);
                    enemy.Damage(weapon.damage, rot);
                }
            }
        }

        StartCoroutine(TimeToFire());
    }

    IEnumerator Reload()
    {
        if (AmmoManager.Instance == null)
        {
            Debug.LogError("¡No hay AmmoManager en la escena!");
            yield break;
        }

        int ammoNeeded = weapon.magazineSize - currentAmmo;
        if (ammoNeeded <= 0)
        {
            Debug.Log("El cargador ya está lleno.");
            yield break;
        }

        // Unificamos la clave de búsqueda en español para que coincida con el AmmoManager
        string tipoTexto = "";
        string nombreArma = weapon.name.ToLower();

        if (nombreArma.Contains("pistol") || nombreArma.Contains("pistola") || weapon == pistolaData)
        {
            tipoTexto = "Pistola";
        }
        else
        {
            tipoTexto = "Escopeta";
        }

        int availableReserve = AmmoManager.Instance.GetAmmo(tipoTexto);
        Debug.Log("Intentando recargar " + weapon.name + " (Clave: " + tipoTexto + ") | Munición en reserva detectada: " + availableReserve);

        if (availableReserve <= 0)
        {
            Debug.Log("¡No hay munición de reserva disponible para " + tipoTexto + "!");
            yield break;
        }

        isReloading = true;
        Debug.Log("Recargando..."); // Mensaje requerido en consola

        yield return new WaitForSeconds(2.0f);

        int ammoToTake = Mathf.Min(ammoNeeded, availableReserve);
        int consumed = AmmoManager.Instance.ConsumeAmmo(tipoTexto, ammoToTake);

        currentAmmo += consumed;
        GuardarBalasArmaActual();

        isReloading = false;
        Debug.Log("¡Recarga completa! Balas en cargador: " + currentAmmo);
    }

    void GuardarBalasArmaActual()
    {
        if (weapon == pistolaData) currentAmmoPistola = currentAmmo;
        else if (weapon == escopetaData) currentAmmoEscopeta = currentAmmo;
    }

    public void CambiarArma()
    {
        if (weapon == null) return;

        GuardarBalasArmaActual();

        bool esPistolaActual = (weapon == pistolaData) || weapon.name.ToLower().Contains("pistol") || weapon.name.ToLower().Contains("pistola");

        if (esPistolaActual && escopetaData != null)
        {
            EquiparArmaNormal(escopetaData);
        }
        else if (pistolaData != null)
        {
            EquiparArmaNormal(pistolaData);
        }
    }

    public void EquiparArmaNormal(Guns nuevaArma)
    {
        if (nuevaArma == null) return;

        GuardarBalasArmaActual();
        weapon = nuevaArma;

        if (weapon == pistolaData)
        {
            if (currentAmmoPistola == -1) currentAmmoPistola = weapon.magazineSize;
            currentAmmo = currentAmmoPistola;
        }
        else if (weapon == escopetaData)
        {
            if (currentAmmoEscopeta == -1) currentAmmoEscopeta = weapon.magazineSize;
            currentAmmo = currentAmmoEscopeta;
        }
        else
        {
            currentAmmo = weapon.magazineSize;
        }

        SetTrigger();
        ActualizarModelosVisibles();

        Debug.Log("Arma equipada: " + weapon.name + " | Balas en cargador: " + currentAmmo);
    }

    void ActualizarModelosVisibles()
    {
        if (weapon == null) return;

        string nombreArma = weapon.name.ToLower();
        string tipoAmmo = weapon.ammoType.ToString().ToLower();

        bool esPistola = nombreArma.Contains("pistol") || tipoAmmo.Contains("pistol") || tipoAmmo.Contains("pistola") || weapon == pistolaData;
        bool esEscopeta = nombreArma.Contains("escopeta") || tipoAmmo.Contains("shotgun") || weapon == escopetaData;

        if (!esPistola && !esEscopeta) esPistola = true;

        if (modeloPistola != null) modeloPistola.SetActive(esPistola);
        if (modeloEscopeta != null) modeloEscopeta.SetActive(esEscopeta);
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            EnemyManager.Instance.AddEnemy(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            EnemyManager.Instance.RemoveEnemy(enemy);
        }
    }

    public void EquiparArmaExpansiva()
    {
        if (escopetaData != null)
        {
            EquiparArmaNormal(escopetaData);
        }
    }
}