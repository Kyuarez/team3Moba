using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class Tower : GameEntity
{
    private List<GameEntity> enemys;
    private bool isAttacking;

    //private Dictionary<int, GameEntity> enemyDic;
    //private int indexTargetEnemy;

    private void OnEnable()
    {
        enemys = new List<GameEntity>();
        isAttacking = false;
        //InitData();
    }

    public override void InitData(EntityData data)
    {
        base.InitData(data);
    }
    void Update()
    {
        float minDistance = 10;
        int targetIndex = -1;
        for (int i = enemys.Count - 1; i >= 0; i--)
        {
            float distance = Vector3.Distance(transform.position, enemys[i].transform.position);
            
            if(distance < minDistance)
            {
                minDistance = distance;
                targetIndex = i;
            }
        }
        //// Update tower logic here
        //// For example, check for enemies in range and attack them
        attackCoolTime = 3f;
        if (targetIndex > -1)   // ������ ���� �ּҰ� ���� �ȴٸ� �ּҸ� ������� Ÿ���� ���Ѵ�.
        {
            GameEntity target = enemys[targetIndex];

            //  Formula �� ���� �޼��带 ȣ���Ѵ�.
            if(isAttacking == false)
            {
                isAttacking = true;
                StartCoroutine(AttackWithCooldown(target));
            }
        }
    }
    IEnumerator AttackWithCooldown(GameEntity target)
    {
        Logger.Log("CoolTime : " + attackCoolTime);
        Formula.attack(this, target);
        yield return new WaitForSeconds(attackCoolTime);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Logger.Log("TriggerEnter : " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            enemys.Add(other.gameObject.GetComponent<GameEntity>());
            //if(indexTargetEnemy < 0)    //   ���� Ÿ���� Ÿ���� ���ٸ�
            //{
            //    indexTargetEnemy = other.gameObject.GetComponent<Enemy>.index;  // ���� �ȿ� ���� ���� �ּҸ� ȹ���Ѵ�.
            //}
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemys.Remove(other.gameObject.GetComponent<GameEntity>());
        }
    }
}

