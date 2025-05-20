using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using System;

public class Champion : GameEntity
{
    private CoolTimeManager coolTime;

    private Animator championAnimator;
    private NavMeshAgent agent;

    private float moveSpeed;
    private float rotateSpeed;
    private float rotateVelocity;

    private float animSmoothTime = 0.1f;

    private float respawnTimeChampion = 5.0f;

    private GameEntity attackTarget;
    private Coroutine autoAttackCoroutine;
    private bool isAttacking = false;

    private int currentLevel;
    private int maxLevel;
    private int currentExp;
    private int requireExp;
    
    private Dictionary<SkillInputType, SkillTable> skillDict;

    //@TK : 차후 MVC 패턴에 맞게 Stat관리하는 별도 클래스 필요 (Level등)
    public event Action OnDeadComplete;
    public event Action<float, float> OnExpChanged;
    public event Action<int> OnLevelChanged;


    public CoolTimeManager PlayerCoolTime => coolTime;
    public int CurrentLevel => currentLevel;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            if (!IsOwner)
            {
                return;
            }

            if (IsHost)
            {
                SetTeam(Team.Red);
                agent.enabled = false;
                transform.position = spawnRedTeamPosition;
                agent.enabled = true;
            }
            else if(IsClient)
            {
                SetTeam(Team.Blue);
                agent.enabled = false;
                transform.position = spawnBlueTeamPosition;
                agent.enabled = true;
            }

            PostNetworkSpawn();
        };

    }

    public void PostNetworkSpawn()
    {
        ChampionTable data = TableManager.Instance.FindTableData<ChampionTable>(entityID);
        InitData(data);
        coolTime = new CoolTimeManager();

        InputManager input = GetComponent<InputManager>();
        input.SetInputManager(this);
        UIManager.Instance.SetMatchUI(this);
    }

    public SkillTable GetSkillData(SkillInputType skillInputType)
    {
        if (skillDict.ContainsKey(skillInputType) == false)
        {
            return null;
        }

        return skillDict[skillInputType];
    }

    protected override void Awake()
    {
        base.Awake();
        championAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        attackTarget = null;
        OnDead += OnDeadAction;
    }

    protected override void Start()
    {
        //
    }

    private void Update()
    {
        championAnimator.SetFloat("MoveFactor", agent.velocity.magnitude / agent.speed, animSmoothTime, Time.deltaTime);
        coolTime?.Update();
    }

    public override void InitData(ChampionTable data)
    {
        base.InitData(data);
        moveSpeed = data.move_speed;
        agent.angularSpeed = 10000f;
        agent.acceleration = 10000f;
        rotateSpeed = 3.0f;


        currentExp = data.current_exp;
        currentLevel = 1;
        maxLevel = data.max_level;
        agent.speed = moveSpeed;
        LevelTable levelTable = TableManager.Instance.FindTableData<LevelTable>(currentLevel);
        requireExp = levelTable.require_exp;

        skillDict = new Dictionary<SkillInputType, SkillTable>();

        for (int i = 0; i < data.skill_list.Count; i++)
        {
            int skillID = data.skill_list[i];
            SkillTable skill = TableManager.Instance.FindTableData<SkillTable>(skillID);
            switch (i)
            {
                case 0:
                    skillDict[SkillInputType.Q] = skill;
                    break;
                case 1:
                    skillDict[SkillInputType.W] = skill;
                    break;
                case 2:
                    skillDict[SkillInputType.E] = skill;
                    break;
                case 3:
                    skillDict[SkillInputType.R] = skill;
                    break;
            }
        }
    }

    public void Move(Vector3 destination)
    {
        //Move
        if (agent.isStopped == true)
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
        if (entity.IsInvincible() == true)
        {
            return;
        }

        attackTarget = entity;
        autoAttackCoroutine = StartCoroutine(CoAutoAttack());
    }

    public void ResetAttackTarget()
    {
        if (autoAttackCoroutine != null)
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
            if (attackTarget == null)
            {
                break;
            }

            float distance = Vector3.Distance(transform.position, attackTarget.transform.position);
            if (distance < attackRange)
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
        //Logger.Log($"Player Attack to {attackTarget.gameObject.name}");
        championAnimator.SetTrigger("OnAttack");
        Attack(Formula.CalcDamage(this), attackTarget);
        yield return new WaitForSeconds(attackCoolTime);
        isAttacking = false;
    }

    private void OnDeadAction()
    {
        Logger.Log("바로 죽음  " + currentHP);
        agent.enabled = false;
        championAnimator.SetTrigger("OnDead");
        //TODO 애니메이션이 끝났다면 실행
        StartCoroutine(CoRespawnChampion());
    }


    IEnumerator CoRespawnChampion()
    {
        yield return new WaitForSeconds(respawnTimeChampion);
        championAnimator.SetTrigger("OnRespawn");
        RespawnChampion();
    }

    private void RespawnChampion()
    {
        currentHP = maxHP;
        Logger.Log("부활  " + currentHP);

        OnDeadComplete?.Invoke();
        agent.enabled = true;
    }

    public void OnGetExpItem(int expAmount)
    {
        if(currentLevel >= maxLevel)
        {
            return;
        }

        currentExp += expAmount;
        if(currentExp >= requireExp)
        {
            currentLevel++;
            OnLevelChanged?.Invoke(currentLevel);
            currentExp -= requireExp;
            
            LevelTable levelTable = TableManager.Instance.FindTableData<LevelTable>(currentLevel);
            requireExp = levelTable.require_exp;
           
            if(currentLevel >= maxLevel)
            {
                currentExp = requireExp;
            }
        }

        OnExpChanged?.Invoke(currentExp, requireExp);
    }

    private Vector3 spawnRedTeamPosition = new Vector3(19f, 6f, 5f);
    private Vector3 spawnBlueTeamPosition = new Vector3(-135f, 6f, -140f);
    public void OnChampionDeadComplete()
    {
        //
        if (GetTeam() == Team.Red)
        {
            transform.position = spawnRedTeamPosition;
        }
        else if (GetTeam() == Team.Blue)
        {
            transform.position = spawnBlueTeamPosition;
        }
    }
} 
