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
    public ArmaOndaExpansiva expansiveWeaponScript; 

    [Header("Inventory & Switching")]
    public List<Guns> weaponInventory = new List<Guns>(); // Lista de armas normales desbloqueadas
    public bool unlockedExpansive = false; // Indica si ya recogió el arma expansiva
    private int currentWeaponIndex = 0; // Índice del arma actual

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gunTrigger = GetComponent<BoxCollider>();

        if (expansiveWeaponScript == null)
        {
            expansiveWeaponScript = GetComponent<ArmaOndaExpansiva>();
        }
    }

    void Start()
    {
        canFire = true;

        // Añade el arma inicial al inventario si se asignó una desde el Inspector
        if (weapon != null && !weaponInventory.Contains(weapon))
        {
            weaponInventory.Add(weapon);
        }
        if (isExpansiveWeapon) unlockedExpansive = true;

        SetTrigger();
    }

    void Update()
    {
        // Detecta el movimiento de la rueda del ratón para cambiar de arma
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SwitchNextWeapon();
        else if (scroll < 0f) SwitchPreviousWeapon();
    }

    // --- LÓGICA DE INVENTARIO Y CAMBIO ---

    // Añade un arma normal al inventario y la equipa
    public void AddNormalWeapon(Guns newWeapon)
    {
        if (!weaponInventory.Contains(newWeapon))
        {
            weaponInventory.Add(newWeapon);
        }
        // Selecciona el índice del arma recién recogida
        currentWeaponIndex = weaponInventory.IndexOf(newWeapon);
        EquipNormalWeapon(newWeapon);
    }

    // Desbloquea el arma expansiva y la equipa
    public void AddExpansiveWeapon()
    {
        unlockedExpansive = true;
        // El arma expansiva siempre será el último índice disponible
        currentWeaponIndex = weaponInventory.Count; 
        EquipExpansiveWeapon();
    }

    private void SwitchNextWeapon()
    {
        int totalWeapons = weaponInventory.Count + (unlockedExpansive ? 1 : 0);
        if (totalWeapons <= 1) return; // No hace nada si solo tiene 1 arma

        currentWeaponIndex++;
        if (currentWeaponIndex >= totalWeapons) currentWeaponIndex = 0;

        EquipWeaponFromIndex();
    }

    private void SwitchPreviousWeapon()
    {
        int totalWeapons = weaponInventory.Count + (unlockedExpansive ? 1 : 0);
        if (totalWeapons <= 1) return;

        currentWeaponIndex--;
        if (currentWeaponIndex < 0) currentWeaponIndex = totalWeapons - 1;

        EquipWeaponFromIndex();
    }

    // Equipa el arma correspondiente según el índice actual
    private void EquipWeaponFromIndex()
    {
        // Si el índice es menor a la cantidad de armas normales, equipa esa
        if (currentWeaponIndex < weaponInventory.Count)
        {
            EquipNormalWeapon(weaponInventory[currentWeaponIndex]);
        }
        // Si el índice es igual a la cantidad de armas normales, significa que es la expansiva
        else if (unlockedExpansive)
        {
            EquipExpansiveWeapon();
        }
    }

    // --- LÓGICA DE EQUIPAMIENTO ORIGINAL ---

    public void SetTrigger()
    {
        if (gunTrigger != null && weapon != null)
        {
            gunTrigger.size = new Vector3(weapon.horizontalRange, weapon.verticalRange, weapon.range);
            gunTrigger.center = new Vector3(0, (0.5f * weapon.verticalRange - 1f), weapon.range * 0.5f);
        }
    }

    public void EquipNormalWeapon(Guns newWeapon)
    {
        weapon = newWeapon;
        isExpansiveWeapon = false;

        if (gunTrigger != null) gunTrigger.enabled = true;
        SetTrigger();
    }
    
    public void EquipExpansiveWeapon()
    {
        isExpansiveWeapon = true;
        if (gunTrigger != null) gunTrigger.enabled = true;
    }

    // --- LÓGICA DE DISPARO ---

    IEnumerator TimeToFire(float waitTime)
    {
        canFire = false;
        yield return new WaitForSeconds(waitTime);
        canFire = true;
    }

    public void Fire()
    {
        if (!canFire) return;

        if (isExpansiveWeapon)
        {
            if (expansiveWeaponScript != null)
            {
                expansiveWeaponScript.ExecuteExpansiveShot(transform, audioSource);
                StartCoroutine(TimeToFire(expansiveWeaponScript.fireRate));
            }
        }
        else
        {
            if (weapon == null) return;

            if (audioSource != null && weapon.sound != null)
            {
                audioSource.PlayOneShot(weapon.sound);
            }

            FireNormalWeapon();
            StartCoroutine(TimeToFire(weapon.fireRate));
        }
    }

    private void FireNormalWeapon()
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
        if (isExpansiveWeapon) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy && EnemyManager.Instance != null) EnemyManager.Instance.AddEnemy(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isExpansiveWeapon) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy && EnemyManager.Instance != null) EnemyManager.Instance.RemoveEnemy(enemy);
    }
}