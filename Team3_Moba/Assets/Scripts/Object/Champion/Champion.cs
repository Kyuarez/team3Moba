using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Champion : GameEntity
{
    private CoolTimeManager coolTime;

    private NetworkAnimator championAnimator;
    private NavMeshAgent agent;

    private float moveSpeed;
    private float rotateSpeed;
    private float rotateVelocity;

    private float animSmoothTime = 0.1f;

    private float respawnTimeChampion = 5.0f;

    private GameEntity attackTarget;
    private Coroutine autoAttackCoroutine;
    private bool isAttacking = false;

    public NetworkVariable<int> currentLevel;
    private int maxLevel;
    public NetworkVariable<int> currentExp;
    private NetworkVariable<int> requireExp;
    private NetworkVariable<float> moveFactor = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Dictionary<SkillInputType, SkillTable> skillDict;

    //@TK : 차후 MVC 패턴에 맞게 Stat관리하는 별도 클래스 필요 (Level등)
    public event Action OnDeadComplete;
    public event Action<float, float> OnExpChanged;
    public event Action<int> OnLevelChanged;


    private Vector3 spawnRedTeamPosition = new Vector3(19f, 6f, 5f);
    private Vector3 spawnBlueTeamPosition = new Vector3(-135f, 6f, -140f);
    public CoolTimeManager PlayerCoolTime => coolTime;
    public int CurrentLevel => currentLevel.Value;
    public int CurrentExp => currentExp.Value;
    public int MaxExp => requireExp.Value;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        agent.enabled = false;
        if (IsHost)
        {
            SetTeam(Team.Red);
            UIUtil.SetCursorTeam(Team.Red);
            transform.position = spawnRedTeamPosition;
        }
        else if (IsClient)
        {
            SetTeam(Team.Blue);
            UIUtil.SetCursorTeam(Team.Blue);
            transform.position = spawnBlueTeamPosition;
        }
        agent.enabled = true;
        
        ChampionTable data = TableManager.Instance.FindTableData<ChampionTable>(entityID);
        InitData(data);

        if (IsOwner)
        {
            UIConnectNet ui = UIManager.Instance.GetOpenedUI<UIConnectNet>();
            UIManager.Instance.CloseUI(ui);
            PostNetworkSpawn();
        }
    }

    public void PostNetworkSpawn()
    {
        InputManager input = GetComponent<InputManager>();
        input.SetInputManager(this);
        coolTime = new CoolTimeManager();
        UIUtil.OnMatchWithChampion(this);
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
        currentLevel = new NetworkVariable<int>(1);
        currentExp = new NetworkVariable<int>(0);
        requireExp = new NetworkVariable<int>(0);
        currentLevel.OnValueChanged += (previous, next) =>
        {
            OnLevelChanged?.Invoke(next);
        };

        currentExp.OnValueChanged += (previous, next) =>
        {
            OnExpChanged?.Invoke(next, requireExp.Value);
        }; 

        requireExp.OnValueChanged += (previous, next) =>
        {
            OnExpChanged?.Invoke(currentExp.Value, next);
        };

        championAnimator = GetComponent<NetworkAnimator>();
        agent = GetComponent<NavMeshAgent>();
        attackTarget = null;
        OnDead += OnDeadAction;
        OnDeadComplete += OnChampionDeadComplete;
    }

    private void Update()
    {
        if (IsOwner)
        {
            float value = agent.velocity.magnitude / agent.speed;
            if (Mathf.Abs(moveFactor.Value - value) > 0.01f)
            {
                moveFactor.Value = value;
            }
        }
        championAnimator.Animator.SetFloat("MoveFactor", moveFactor.Value, animSmoothTime, Time.deltaTime);
        coolTime?.Update();
    }

    public override void InitData(ChampionTable data)
    {
        base.InitData(data);
        moveSpeed = data.move_speed;
        agent.angularSpeed = 10000f;
        agent.acceleration = 10000f;
        rotateSpeed = 3.0f;

        SetExp(data.current_exp);
        SetLevel(1);
        maxLevel = data.max_level;
        agent.speed = moveSpeed;
        LevelTable levelTable = TableManager.Instance.FindTableData<LevelTable>(currentLevel.Value);
        SetMaxExp(levelTable.require_exp);

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
            if (attackTarget == null || attackTarget.GetHP() <= 0)
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
        championAnimator.SetTrigger("OnAttack");
        Attack(Formula.CalcDamage(this), attackTarget);
        SoundManager.Instance.PlaySFX(2);
        EffectManager.Instance.PlayEffect(1, attackTarget.transform.position,new Vector3(1,1,1), Quaternion.identity);
        yield return new WaitForSeconds(attackCoolTime);
        isAttacking = false;
    }

    private void OnDeadAction()
    {
        agent.enabled = false;
        SoundManager.Instance.PlaySFX(5);
        championAnimator.SetTrigger("OnDead");
        //TODO 애니메이션이 끝났다면 실행
        StartCoroutine(CoRespawnChampion());
    }


    IEnumerator CoRespawnChampion()
    {
        yield return new WaitForSeconds(respawnTimeChampion);
        championAnimator.SetTrigger("OnRespawn");
        OnDeadComplete?.Invoke();
    }
    public void OnChampionDeadComplete()
    {
        SetHP(maxHP.Value);

        if (GetTeam() == Team.Red)
        {
            transform.position = spawnRedTeamPosition;
        }
        else if (GetTeam() == Team.Blue)
        {
            transform.position = spawnBlueTeamPosition;
        }
        agent.enabled = true;
    }

    public void OnGetExpItem(int expAmount)
    {
        int calcExp = 0;
        if(currentLevel.Value >= maxLevel)
        {
            return;
        }

        SoundManager.Instance.PlaySFX(7);
        calcExp = currentExp.Value + expAmount;
        if(calcExp >= requireExp.Value)
        {
            int nextLevel = currentLevel.Value + 1;
            ChampionTable championData = TableManager.Instance.FindTableData<ChampionTable>(entityID);
            //TODO Level에 따른 스탯 변경
            SetLevel(nextLevel);
            SetMaxHP(Formula.CalcHP(championData.hp, nextLevel));
            SetHP(Formula.CalcHP(championData.hp, nextLevel));
            attackDamage = Formula.CalcAttack(championData.attack, nextLevel);
            recoveryAmount = Formula.CalcRecovery(championData.recovery, nextLevel);

            OnLevelChanged?.Invoke(nextLevel);
            int calcRestExp = calcExp - requireExp.Value;
            SetExp(calcRestExp);
            
            LevelTable levelTable = TableManager.Instance.FindTableData<LevelTable>(nextLevel);
            SetMaxExp(levelTable.require_exp);
           
            if(nextLevel >= maxLevel)
            {
                calcExp = levelTable.require_exp;
                SetExp(requireExp.Value);
            }
        }
        else
        {
            SetExp(calcExp);
        }
    }


    public void SetExp(int exp)
    {
        if (!IsOwner)
        {
            return;
        }
        ServerSetExpRpc(exp);
    }
    [Rpc(SendTo.Server)]
    public void ServerSetExpRpc(int exp)
    {
        if (IsServer)
        {
            currentExp.Value = exp;
        }
    }
    public void SetMaxExp(int exp)
    {
        if (!IsOwner)
        {
            return;
        }
        ServerSetMaxExpRpc(exp);
    }
    [Rpc(SendTo.Server)]
    public void ServerSetMaxExpRpc(int exp)
    {
        if (IsServer)
        {
            requireExp.Value = exp;
        }
    }

    public void SetLevel(int level)
    {
        if (!IsOwner)
        {
            return;
        }
        ServerSetLevelRpc(level);
    }
    [Rpc(SendTo.Server)]
    public void ServerSetLevelRpc(int level)
    {
        if (IsServer)
        {
            currentLevel.Value = level;
        }
    }
} 
