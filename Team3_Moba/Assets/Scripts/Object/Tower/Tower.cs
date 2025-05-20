using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;

public enum TowerLevelType
{
    Level1,
    Level2, 
    Nexus,
}

public class Tower : GameEntity
{
    //Test
    [SerializeField] private TowerLevelType levelType;
    [SerializeField] private GameObject projectileObj;

    private Transform projectileTransform;
    private List<GameEntity> enemys;
    private bool isAttacking;
    private List<Tower> invincibleConditionList;

    protected override void Awake()
    {
        base.Awake();
        projectileTransform = transform.Find("ProjectileTransform");

        enemys = new List<GameEntity>();
        isAttacking = false;
        GameEntity entity = GetComponent<GameEntity>();
        entity.OnDead += OnTowerDestroyed;

    }

    protected override void Start()
    {
        base.Start();
        SetInvincibleCondition();

        if (invincibleConditionList != null && invincibleConditionList.Count > 0)
        {
            isInvincible = true;
        }
    }

    public override void InitData(EntityTable data)
    {
        base.InitData(data);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetTeam(Team.Red);
    }

    void Update()
    {
        //@tk : GetTeam을 제대로 받지 않음 주석처리 (차후 확인)
        //UpdateCurrentEnemys();
        //AttackClosestTarget();
    }

    private void SetInvincibleCondition()
    {
        invincibleConditionList = new List<Tower>();
        Tower[] towers = UnityEngine.Object.FindObjectsByType<Tower>(FindObjectsSortMode.None);
        List<Tower> teamTowers = new List<Tower>();
        foreach (Tower tower in towers)
        {
            if (IsOpposingTeam(tower))
            {
                continue;
            }

            teamTowers.Add(tower);
        }

        if (levelType == TowerLevelType.Nexus)
        {
            foreach (Tower tower in teamTowers)
            {
                if(tower.levelType == TowerLevelType.Level2)
                {
                    invincibleConditionList.Add(tower);
                    tower.OnDead += () =>
                    {
                        invincibleConditionList.Remove(tower);
                    };
                }
            }
        }
        else if(levelType == TowerLevelType.Level2)
        {
            foreach (Tower tower in teamTowers)
            {
                if (tower.levelType == TowerLevelType.Level1)
                {
                    invincibleConditionList.Add(tower);
                    tower.OnDead += () => 
                    {
                        invincibleConditionList.Remove(tower);
                    };
                }
            }
        }
    }

    private void UpdateCurrentEnemys()
    {
        if(isInvincible == true && levelType == TowerLevelType.Nexus)
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
        if (isInvincible == true && levelType == TowerLevelType.Nexus)
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
        //// Update tower logic here
        //// For example, check for enemies in range and attack them
        attackCoolTime = 3f;
        if (targetIndex > -1)   // 근접한 적의 주소가 감지 된다면 주소를 기반으로 타겟을 정한다.
        {
            GameEntity target = enemys[targetIndex];

            //  Formula 의 공격 메서드를 호출한다.
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
        Logger.Log("CoolTime : " + attackCoolTime);
        //TODO 투사체 보내기
        ReqProjectileServerRpc();
        yield return new WaitForSeconds(attackCoolTime);
        isAttacking = false;
    }

    [ServerRpc]
    private void ReqProjectileServerRpc()
    {
        if (IsServer)
        {
            ShotProjectileClientRpc();
        }
    }

    [ClientRpc]
    private void ShotProjectileClientRpc()
    {
        Projectile projectile = Instantiate(projectileObj, projectileTransform.position, Quaternion.identity).AddComponent<Projectile>();
        float damage = Formula.CalcDamage(this);
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

