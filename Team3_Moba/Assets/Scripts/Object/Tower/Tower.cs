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
        //@tk : GetTeam�� ����� ���� ���� �ּ�ó�� (���� Ȯ��)
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

        // ���� ������ �� �߰� (Enter ����)
        foreach (var enemy in detectedEnemies)
        {
            if (!enemys.Contains(enemy))
            {
                enemys.Add(enemy);
            }
        }

        // ���� ����Ʈ���� ����� �� ���� (Exit ����)
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
        if (targetIndex > -1)   // ������ ���� �ּҰ� ���� �ȴٸ� �ּҸ� ������� Ÿ���� ���Ѵ�.
        {
            GameEntity target = enemys[targetIndex];

            //  Formula �� ���� �޼��带 ȣ���Ѵ�.
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
        //TODO ����ü ������
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

