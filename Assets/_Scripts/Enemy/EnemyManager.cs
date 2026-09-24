using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public List<Enemy> enemiesInRange = new List<Enemy>();

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
    }

    public void AddEnemy(Enemy enemy)
    {

        enemiesInRange.Add(enemy);

    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemiesInRange.Remove(enemy);
    }

}