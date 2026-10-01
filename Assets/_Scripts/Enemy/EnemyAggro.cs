using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggro : MonoBehaviour
{
    public bool isAggro;

    public float distanceToAggro;

    [HideInInspector] public Transform playerTransform;

    private EnemyAttack enemyAttack;

    // Start is called before the first frame update
    void Start()
    {
        if(playerTransform == null)
        {
            playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        }
        enemyAttack = GetComponentInChildren<EnemyAttack>();
        isAggro = false;
    }


    // Update is called once per frame
    void Update()
    {
        CheckEnemyAggro();
    }

    public void CheckEnemyAggro()
    {
        var dis = Vector3.Distance(transform.position, playerTransform.position);
        if (dis > distanceToAggro)
        {
            isAggro = false;
        }
        else
        {
            isAggro = true;
        }
    }

    public void EnemyDamage()
    {
        if (enemyAttack.isAttacking)
        {
            playerTransform.GetComponent<PlayerStats>().PlayerDamage();
        }
    }
}
