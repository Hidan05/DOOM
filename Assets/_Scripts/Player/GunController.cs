using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public static GunController Instance;

    [Header("General Configuration")]
    private BoxCollider gunTrigger;
    public Guns weapon; 
    public LayerMask raycastLayerMask;
    public AudioSource audioSource;
    private bool canFire;

    [Header("Weapon Mode")]
    public bool isExpansiveWeapon = false;
    public ArmaOndaExpansiva expansiveWeaponScript; // Referencia al script independiente del arma expansiva

    void Awake()
    {
        // Patrón Singleton: Garantiza que solo exista una instancia del controlador en la escena
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gunTrigger = GetComponent<BoxCollider>();

        // Intenta obtener la referencia automáticamente si no fue asignada en el Inspector
        if (expansiveWeaponScript == null)
        {
            expansiveWeaponScript = GetComponent<ArmaOndaExpansiva>();
        }
    }

    void Start()
    {
        canFire = true;
        SetTrigger();
    }

    // Ajusta las dimensiones y posición del BoxCollider según los rangos definidos en el arma normal
    public void SetTrigger()
    {
        if (gunTrigger != null && weapon != null)
        {
            gunTrigger.size = new Vector3(weapon.horizontalRange, weapon.verticalRange, weapon.range);
            gunTrigger.center = new Vector3(0, (0.5f * weapon.verticalRange - 1f), weapon.range * 0.5f);
        }
    }

    /// <summary>
    /// Sustituye el arma normal actual por una nueva y actualiza el área de detección
    /// </summary>
    public void EquipNormalWeapon(Guns newWeapon)
    {
        weapon = newWeapon;
        isExpansiveWeapon = false;

        if (gunTrigger != null) 
        {
            gunTrigger.enabled = true;
        }
        SetTrigger();
    }
    
    // Cambia el modo de disparo para usar el sistema de onda expansiva
    public void EquipExpansiveWeapon()
    {
        isExpansiveWeapon = true;

        if (gunTrigger != null) 
        {
            gunTrigger.enabled = true;
        }
    }

    // Corrutina para manejar la cadencia de tiro, bloqueando y liberando el disparo según el tiempo de espera
    IEnumerator TimeToFire(float waitTime)
    {
        canFire = false;
        yield return new WaitForSeconds(waitTime);
        canFire = true;
    }

    // Método principal de disparo evaluado según el tipo de arma equipada
    public void Fire()
    {
        if (!canFire) return;

        if (isExpansiveWeapon)
        {
            if (expansiveWeaponScript != null)
            {
                // Delega la ejecución del disparo y sus físicas al script especializado en ondas
                expansiveWeaponScript.ExecuteExpansiveShot(transform, audioSource);
                StartCoroutine(TimeToFire(expansiveWeaponScript.fireRate));
            }
        }
        else
        {
            if (weapon == null) return;

            // Reproduce el sonido específico asignado al arma normal
            if (audioSource != null && weapon.sound != null)
            {
                audioSource.PlayOneShot(weapon.sound);
            }

            FireNormalWeapon();
            StartCoroutine(TimeToFire(weapon.fireRate));
        }
    }

    // Aplica daño a los enemigos dentro del área mediante proyección de rayos (Raycast)
    private void FireNormalWeapon()
    {
        if (EnemyManager.Instance == null) return;

        // Itera sobre la lista de enemigos actualmente detectados en el rango
        foreach (Enemy enemy in EnemyManager.Instance.enemiesInRange)
        {
            if (enemy == null) continue;

            RaycastHit hit;
            var direction = enemy.transform.position - transform.position;

            // Lanza un rayo hacia el enemigo para comprobar si hay obstáculos en la trayectoria
            if (Physics.Raycast(transform.position, direction, out hit, weapon.range * 1.5f, raycastLayerMask))
            {
                // Si el rayo impacta directamente al enemigo, procede a aplicar el daño
                if (hit.transform == enemy.transform)
                {
                    enemy.Damage(weapon.damage, transform.rotation);
                }
            }
        }
    }

    // Registra a los enemigos que ingresan al área de alcance del arma normal
    private void OnTriggerEnter(Collider other)
    {
        if (isExpansiveWeapon) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.AddEnemy(enemy);
        }
    }

    // Elimina del registro a los enemigos que abandonan el área de alcance del arma normal
    private void OnTriggerExit(Collider other)
    {
        if (isExpansiveWeapon) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RemoveEnemy(enemy);
        }
    }
}