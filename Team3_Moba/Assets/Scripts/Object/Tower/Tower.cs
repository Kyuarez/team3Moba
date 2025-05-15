using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class Tower : GameEntity
{
    //Test
    [SerializeField] private GameObject projectileObj;

    private Transform projectileTransform;
    private List<GameEntity> enemys;
    private bool isAttacking;

    //private Dictionary<int, GameEntity> enemyDic;
    //private int indexTargetEnemy;
    private void Awake()
    {
        EntityData data = new EntityData();
        InitData(data);

        projectileTransform = transform.Find("ProjectileTransform");
    }

    private void OnEnable()
    {
        enemys = new List<GameEntity>();
        isAttacking = false;
        GameEntity entity = GetComponent<GameEntity>();
        entity.OnDead += OnTowerDestroyed;
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
    IEnumerator CoAttackWithCooldown(GameEntity target)
    {
        Logger.Log("CoolTime : " + attackCoolTime);
        //TODO 투사체 보내기
        Projectile projectile = Instantiate(projectileObj, projectileTransform.position, Quaternion.identity).AddComponent<Projectile>();
        projectile.InitProjectile(ProjectileType.Guided, target, 10f, 10f, () => Attack(this, target));

        //Attack(this, target);
        yield return new WaitForSeconds(attackCoolTime);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Logger.Log("TriggerEnter : " + other.gameObject.name);
        if (gameObject.GetComponent<GameEntity>().GetTeam()!= other.gameObject.GetComponent<GameEntity>().GetTeam())
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
        if (other.gameObject.CompareTag("Player"))
        {
            enemys.Remove(other.gameObject.GetComponent<GameEntity>());
        }
    }

    void OnTowerDestroyed()
    {
        Destroy(gameObject);
    }

}

