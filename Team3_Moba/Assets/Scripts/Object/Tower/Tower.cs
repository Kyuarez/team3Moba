using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;

public enum TowerType
{
    Tower,
    TwinTower, 
    Nexus,
}

public class Tower : GameEntity
{
    [SerializeField] private Team initTeam;
    [SerializeField] private TowerType towerType;
    [SerializeField] private List<Tower> invincibleConditionList;

    private List<GameEntity> enemys;
    private bool isAttacking;

    protected override void Awake()
    {
        base.Awake();
        enemys = new List<GameEntity>();
        isAttacking = false;
        GameEntity entity = GetComponent<GameEntity>();
        entity.OnDead += OnTowerDestroyed;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetTeam(initTeam);
        EntityTable data = TableManager.Instance.FindTableData<EntityTable>(entityID);
        InitData(data);

        if (invincibleConditionList != null && invincibleConditionList.Count > 0)
        {
            isInvincible = true;
            foreach (Tower tower in invincibleConditionList)
            {
                tower.OnDead += () =>
                {
                    invincibleConditionList.Remove(tower);
                };
            }
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        UpdateCurrentEnemys();
        AttackClosestTarget();
    }

    private void UpdateCurrentEnemys()
    {

        if(isInvincible == true && towerType == TowerType.Nexus)
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        List<GameEntity> detectedEnemies = new List<GameEntity>();

        foreach (var hit in hits)
        {
            GameEntity entity = hit.GetComponent<GameEntity>();
            if (entity != null && IsOpposingTeam(entity))
            {
                detectedEnemies.Add(entity);
            }
        }

        // 새로 감지된 적 추가 (Enter 역할)
        foreach (var enemy in detectedEnemies)
        {
            if (!enemys.Contains(enemy))
            {
                enemys.Add(enemy);
            }
        }

        // 기존 리스트에서 사라진 적 제거 (Exit 역할)
        for (int i = enemys.Count - 1; i >= 0; i--)
        {
            if (!detectedEnemies.Contains(enemys[i]))
            {
                enemys.RemoveAt(i);
            }
        }
    }

    private void AttackClosestTarget()
    {
        if (isInvincible == true && towerType == TowerType.Nexus)
        {
            return;
        }

        float minDistance = 10;
        int targetIndex = -1;
        for (int i = enemys.Count - 1; i >= 0; i--)
        {
            float distance = Vector3.Distance(transform.position, enemys[i].transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                targetIndex = i;
            }
        }
        
        attackCoolTime = 3f;
        if (targetIndex > -1)   // 근접한 적의 주소가 감지 된다면 주소를 기반으로 타겟을 정한다.
        {
            GameEntity target = enemys[targetIndex];
            if(target.GetHP() <= 0)
            {
                return;
            }

            if (isAttacking == false)
            {
                isAttacking = true;
                StartCoroutine(CoAttackWithCooldown(target));
            }
        }
    }

    public override bool IsInvincible()
    {
        if (invincibleConditionList != null && invincibleConditionList.Count > 0)
        {
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
        }

        return isInvincible;
    }

    IEnumerator CoAttackWithCooldown(GameEntity target)
    {
        ServerShootRpc(target.NetworkObjectId, "cannon");
        yield return new WaitForSeconds(attackCoolTime);
        isAttacking = false;
    }

    void OnTowerDestroyed()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float hitRadius = attackRange;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }

}

