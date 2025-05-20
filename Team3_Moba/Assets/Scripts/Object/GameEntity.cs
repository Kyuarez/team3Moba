using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


public class GameEntity : NetworkBehaviour 
{ 
    [SerializeField] protected int entityID;
    
    [SerializeReference] protected NetworkVariable<Team> team;

    //  attack variable
    protected float attackDamage;
    protected float attackRange;
    protected float attackCoolTime;
    protected float attackDelay;

    //  health variable
    protected float maxHP;
    protected float currentHP;
    protected bool isInvincible = false;

    // move variable - 보류 태규님과 상의 후 결정
    //@TK : 차후 MVC 패턴에 맞게 Stat관리하는 별도 클래스 필요
    public event Action<float, float> OnHPChanged;
    public event Action OnDead;

    protected float recoveryDelay = 10f;         // 회복 코루틴을 닿는데 필요한 시간( 해당 시간 만큼 데미지를 받지 않아야지 회복)
    protected float recoveryAmount;              // 초당 회복력
    private float damagedTime;                   //마지막으로 데미지 입은 시간
    private Coroutine recoveryCoroutine;

    protected virtual void Awake()
    {
        team = new NetworkVariable<Team>(Team.None);
    }

    protected virtual void Start()
    {
        EntityTable data = TableManager.Instance.FindTableData<EntityTable>(entityID);
        InitData(data);
    }

    public int GetEntityID()
    {
        return entityID;
    }

    public virtual void InitData(EntityTable data)
    {
        this.attackDamage = data.damage;
        this.attackRange = data.attack_range;
        this.attackCoolTime = data.attack_cool_time;

        this.maxHP = data.hp;
        this.currentHP = maxHP;

    }
    public virtual void InitData(ChampionTable data)
    {
        this.attackDamage = data.attack;
        this.attackRange = data.attack_range;

        this.maxHP = data.hp;
        this.currentHP = maxHP;
        this.recoveryAmount = data.recovery;
    }

    public float GetHP()
    {
        return currentHP;
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void Heal(float hpValue) // 힐을 받았을때
    {
        currentHP = Mathf.Min(maxHP, currentHP + hpValue);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(float damageValue) // 데미지를 받았을때
    {
        //Logger.Log("공격 : " + damageValue);
        currentHP = Mathf.Max(0, currentHP - damageValue);
        OnHPChanged?.Invoke(currentHP, maxHP);

        //사망 처리
        if (currentHP <= 0)
        {
            OnDead?.Invoke();
        }


        damagedTime = Time.time;
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }

        recoveryCoroutine = StartCoroutine(coRecoveryCoroutine());
    }

    public void Attack(float damage, GameEntity target)
    {
        if(target == null)
        {
            return;
        }
        if (target.IsInvincible() == true)
        {
            return;
        }

        target.TakeDamage(damage);
    }

    public virtual bool IsInvincible()
    {
        return isInvincible;
    }

    public Team GetTeam()
    {
        return team.Value;
    }
    public void SetTeam(Team team)
    {
        if (!IsOwner)
        {
            return;
        }

        ReqSetTeamServerRpc(team);
    }
    public bool IsOpposingTeam(GameEntity other)
    {
        return team.Value != other.GetTeam();
    }

    [ServerRpc]
    public void ReqSetTeamServerRpc(Team team)
    {
        if (IsServer)
        {
            AckSetTeamClientRpc(team);
        }
    }

    [ClientRpc]
    public void AckSetTeamClientRpc(Team team)
    {
        this.team.Value = team;
    }

    public IEnumerator coRecoveryCoroutine()
    {
        while (true)
        {
            if (Time.time - damagedTime < recoveryDelay || currentHP >= maxHP)
            {
                yield return null;
                continue;
            }

            currentHP += recoveryAmount * Time.deltaTime;
            currentHP = Mathf.Min(currentHP, maxHP);
            OnHPChanged?.Invoke(currentHP, maxHP);

            yield return null;
        }
    }
}
