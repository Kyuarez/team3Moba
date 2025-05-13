using UnityEngine;
using System.Collections.Generic;

public class Tower : GameEntity
{
    public AttackComponent Attack;
    public HealthComponent Health;
    //private Dictionary<int, GameEntity> enemyDic;
    private List<GameEntity> enemys;
    //private int indexTargetEnemy;

    private void OnEnable()
    {
        Attack = new AttackComponent();
        Health = new HealthComponent();
    }

    public override void InitData(EntityData data)
    {
        base.InitData(data);

        // Initialize tower-specific data here
        // For example, set the tower's attack damage, range, etc.
        Attack.SetData(data);
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
        if (targetIndex > -1)   // ������ ���� �ּҰ� ���� �ȴٸ� �ּҸ� ������� Ÿ���� ���Ѵ�.
        {
            GameEntity target = enemys[targetIndex];
            Attack.Attack(target);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
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
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemys.Remove(other.gameObject.GetComponent<GameEntity>());
        }
    }
}

