using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public static GunController Instance;
    private BoxCollider gunTrigger;
    public Guns weapon;
    public LayerMask raycastLayerMask;
    private bool canFire;
    public AudioSource audioSource;

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
    }

    // Start is called before the first frame update
    void Start()
    {

        canFire = true;
        SetTrigger();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTrigger()
    {
        gunTrigger.size = new Vector3(weapon.horizontalRange, weapon.verticalRange, weapon.range);
        gunTrigger.center = new Vector3(0, (0.5f * weapon.verticalRange - 1f), weapon.range * 0.5f);
    }

    IEnumerator Timetofire()
    {
        canFire = false;
        yield return new WaitForSeconds(weapon.fireRate);
        canFire = true;
    }

    public void Fire()
    {
        if (canFire)
        {
            audioSource.PlayOneShot(weapon.sound);
            foreach (Enemy enemy in EnemyManager.Instance.enemiesInRange)
            {
                RaycastHit hit;

                var direction = enemy.transform.position - transform.position;

                if (Physics.Raycast(transform.position, direction, out hit, weapon.range * 1.5f, raycastLayerMask))
                {
                    if (hit.transform == enemy.transform)
                    {
                        Quaternion rot = Quaternion.LookRotation(-hit.normal);
                        enemy.Damage(weapon.damage, transform.rotation);
                        enemy.Die();
                    }
                    if (hit.transform.gameObject.CompareTag("Enemy"))
                    {
                        Debug.Log("Explion");
                    }
                }
            }

            StartCoroutine(Timetofire());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        EnemyManager.Instance.AddEnemy(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        EnemyManager.Instance.RemoveEnemy(enemy);
    }
}
