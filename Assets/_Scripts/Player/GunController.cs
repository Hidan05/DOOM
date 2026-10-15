using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public static GunController Instance;

    [Header("Configuración General")]
    private BoxCollider gunTrigger;
    public Guns weapon; // ScriptableObject del arma normal
    public LayerMask raycastLayerMask;
    public AudioSource audioSource;
    private bool canFire;

    [Header("Modo de Arma")]
    public bool isExpansionWeapon = false;
    public ArmaOndaExpansiva armaOndaScript; // Referencia al script independiente

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gunTrigger = GetComponent<BoxCollider>();

        // Intenta obtener el componente si no está asignado en el Inspector
        if (armaOndaScript == null)
        {
            armaOndaScript = GetComponent<ArmaOndaExpansiva>();
        }
    }

    void Start()
    {
        canFire = true;
        SetTrigger();
    }

    public void SetTrigger()
    {
        if (gunTrigger != null && weapon != null)
        {
            gunTrigger.size = new Vector3(weapon.horizontalRange, weapon.verticalRange, weapon.range);
            gunTrigger.center = new Vector3(0, (0.5f * weapon.verticalRange - 1f), weapon.range * 0.5f);
        }
    }

    /// <summary>
    /// Equipa un arma normal desde un ScriptableObject
    /// </summary>
    public void EquiparArmaNormal(Guns nuevaArma)
    {
        weapon = nuevaArma;
        isExpansionWeapon = false;

        if (gunTrigger != null) gunTrigger.enabled = true;
        SetTrigger();
    }
    
public void EquiparArmaExpansiva()
{
    isExpansionWeapon = true;

    // Mantenemos el BoxCollider activo para no perder detecciones de física
    if (gunTrigger != null) 
    {
        gunTrigger.enabled = true;
    }
}

    IEnumerator Timetofire(float tiempoEspera)
    {
        canFire = false;
        yield return new WaitForSeconds(tiempoEspera);
        canFire = true;
    }

    public void Fire()
    {
        if (!canFire) return;

        if (isExpansionWeapon)
        {
            if (armaOndaScript != null)
            {
                // Dispara desde la posición del arma y delega la física a ArmaOndaExpansiva
                armaOndaScript.EjecutarDisparoOnda(transform, audioSource);
                StartCoroutine(Timetofire(armaOndaScript.cadenciaDisparo));
            }
        }
        else
        {
            if (weapon == null) return;

            if (audioSource != null && weapon.sound != null)
            {
                audioSource.PlayOneShot(weapon.sound);
            }

            DispararArmaNormal();
            StartCoroutine(Timetofire(weapon.fireRate));
        }
    }

    private void DispararArmaNormal()
    {
        if (EnemyManager.Instance == null) return;

        foreach (Enemy enemy in EnemyManager.Instance.enemiesInRange)
        {
            if (enemy == null) continue;

            RaycastHit hit;
            var direction = enemy.transform.position - transform.position;

            if (Physics.Raycast(transform.position, direction, out hit, weapon.range * 1.5f, raycastLayerMask))
            {
                if (hit.transform == enemy.transform)
                {
                    enemy.Damage(weapon.damage, transform.rotation);
                    enemy.Die();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isExpansionWeapon) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.AddEnemy(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isExpansionWeapon) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RemoveEnemy(enemy);
        }
    }
}