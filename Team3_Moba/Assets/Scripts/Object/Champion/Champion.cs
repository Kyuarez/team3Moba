using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Champion : GameEntity
{
    private Animator championAnimator;
    private NavMeshAgent agent;

    private float moveSpeed;
    private float rotateSpeed;
    private float rotateVelocity;

    private float animSmoothTime = 0.1f;

    private GameEntity attackTarget;
    private Coroutine autoAttackCoroutine;
    private bool isAttacking = false;

    private Dictionary<SkillInputType, SkillData> skillDict;

    public SkillData GetSkillData(SkillInputType skillInputType)
    {
        if(skillDict.ContainsKey(skillInputType) == false)
        {
            return null;
        }

        return skillDict[skillInputType];
    }

    private void Awake()
    {
        //Todo : 지금은 가짜 데이터, 테이블 넣으면, 테이블에서 정보 값 가져와서 넣기
        EntityData data = new EntityData();
        InitData(data);
    }

    private void OnEnable()
    {
        championAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        attackTarget = null;

        #region Test
        //@tk : agent 세팅 test
        moveSpeed = 10f;
        agent.speed = moveSpeed;
        agent.angularSpeed = 10000f;
        agent.acceleration = 10000f;

        //@tk 임시 스킬 넣기
        skillDict = new Dictionary<SkillInputType, SkillData>();
        SkillData fireball = new SkillData()
        {
            SkillID = 1,
            PoolPath = "TestFireball",
            SkillExecuteType = SkillExecuteType.SetTarget,
            SkillActionType = SkillActionType.Launch,
        };
        skillDict.Add(SkillInputType.Q, fireball);
        #endregion
    }

    private void Update()
    {
        championAnimator.SetFloat("MoveFactor", agent.velocity.magnitude / agent.speed, animSmoothTime, Time.deltaTime);
    }

    public override void InitData(EntityData data)
    {
        this.attackDamage = 50f;
        this.attackRange = 3f;
        this.attackCoolTime = 1.5f;
        this.attackDelay = 1f;

        this.maxHP = 100;
        this.currentHP = maxHP;
    }

    public void Move(Vector3 destination)
    {
        //Move
        if(agent.isStopped == true)
        {
            agent.isStopped = false;
        }
        agent.SetDestination(destination);
        agent.stoppingDistance = 0;

        //Rotate
        Quaternion rotationToLookat = Quaternion.LookRotation(destination - transform.position);
        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookat.eulerAngles.y, ref rotateVelocity, rotateSpeed * 5f * Time.deltaTime);

        transform.eulerAngles = new Vector3(0f, rotationY, 0f);
    }
    public void StopMove()
    {
        if (agent.isStopped == false) 
        {
            agent.isStopped = true;
        }
    }

    public void SetAttackTarget(GameEntity entity)
    {
        attackTarget = entity;
        autoAttackCoroutine = StartCoroutine(CoAutoAttack());
    }

    public void ResetAttackTarget()
    {
        if(autoAttackCoroutine != null)
        {
            StopCoroutine(autoAttackCoroutine);
            autoAttackCoroutine = null;
        }

        attackTarget = null;
    }

    private IEnumerator CoAutoAttack()
    {
        //@tk : 플레이어와 타겟과의 거리
        while (true)
        {
            if(attackTarget == null)
            {
                break;
            }

            float distance = Vector3.Distance(transform.position, attackTarget.transform.position);
            if(distance < attackRange)
            {
                StopMove();
                if (isAttacking == false)
                {
                    StartCoroutine(CoAttackWithCoolTime());
                }
            }
            else
            {
                Move(attackTarget.transform.position);
            }   
            yield return null;
        }
    }

    private IEnumerator CoAttackWithCoolTime()
    {
        isAttacking = true;
        Logger.Log($"Player Attack to {attackTarget.gameObject.name}");
        championAnimator.SetTrigger("OnAttack");
        Attack(this, attackTarget);
        yield return new WaitForSeconds(attackCoolTime);
        isAttacking = false;
    }
}
