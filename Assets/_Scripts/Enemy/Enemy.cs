using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void update()
    {
        Die();
    }

    public int health;
    public GameObject blood;

    public void Damage(int damage, Quaternion rot)
    {
        health -= damage;

        GameObject bloodObject = Instantiate(blood, transform.position, rot);
        Destroy(bloodObject, 0.5f);
    }

    public void Die()
    {
        if (health <= 0)
        {
            EnemyManager.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }
}
