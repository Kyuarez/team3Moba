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
        if (targetIndex > -1)   // 근접한 적의 주소가 감지 된다면 주소를 기반으로 타겟을 정한다.
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
            //if(indexTargetEnemy < 0)    //   만약 타워의 타겟이 없다면
            //{
            //    indexTargetEnemy = other.gameObject.GetComponent<Enemy>.index;  // 범위 안에 들어온 적의 주소를 획득한다.
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

