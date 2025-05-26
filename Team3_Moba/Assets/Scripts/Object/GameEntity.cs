using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


public class GameEntity : NetworkBehaviour 
{ 
    [SerializeField] protected int entityID;
    
    [SerializeReference] protected NetworkVariable<Team> team;
    //  health variable
    [SerializeReference] protected NetworkVariable<float> maxHP;
    [SerializeReference] protected NetworkVariable<float> currentHP;

    //  attack variable
    protected float attackDamage;
    protected float attackRange;
    protected float attackCoolTime;
    protected float attackDelay;

    protected bool isInvincible = false;

    // move variable - 보류 태규님과 상의 후 결정
    //@TK : 차후 MVC 패턴에 맞게 Stat관리하는 별도 클래스 필요
    public event Action<float, float> OnHPChanged;
    public event Action OnDead;

    protected float recoveryDelay = 10f;         // 회복 코루틴을 닿는데 필요한 시간( 해당 시간 만큼 데미지를 받지 않아야지 회복)
    protected float recoveryAmount;              // 초당 회복력
    private float damagedTime;                   //마지막으로 데미지 입은 시간
    private Coroutine recoveryCoroutine;

    protected Transform projectileTransform;

    protected virtual void Awake()
    {
        projectileTransform = transform.Find("ProjectileTransform");

        team = new NetworkVariable<Team>(Team.None);
        maxHP = new NetworkVariable<float>(0f);
        currentHP = new NetworkVariable<float>(0f);

        maxHP.OnValueChanged += (previous, next) =>
        {
            OnHPChanged?.Invoke(currentHP.Value, next);
        };

        currentHP.OnValueChanged += (previous, next) =>
        {
            OnHPChanged?.Invoke(next, maxHP.Value);
        };
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

        SetMaxHP(data.hp);
        SetHP(data.hp);
    }
    public virtual void InitData(ChampionTable data)
    {
        this.attackDamage = data.attack;
        this.attackRange = data.attack_range;
        this.attackCoolTime = data.attack_cool_time;

        SetMaxHP(data.hp);
        SetHP(data.hp);

        this.recoveryAmount = data.recovery;
    }



    public float GetHP()
    {
        return currentHP.Value;
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void Heal(float hpValue) // 힐을 받았을때
    {
        float hpData = Mathf.Min(currentHP.Value + hpValue, maxHP.Value);
        SetHP(hpData);
    }
    public void SetHP(float hp)
    {
        if (!IsOwner)
        {
            return;
        }
        ServerSetHpRpc(hp);
    }
    [Rpc(SendTo.Server)]
    public void ServerSetHpRpc(float hp)
    {
        if (IsServer)
        {
            currentHP.Value = hp;
        }
    }

    public void SetMaxHP(float hp)
    {
        if (!IsOwner)
        {
            return;
        }
        ServerSetMaxHpRpc(hp);
    }
    [Rpc(SendTo.Server)]
    public void ServerSetMaxHpRpc(float hp)
    {
        if (IsServer)
        {
            maxHP.Value = hp;
        }
    }

    public void TakeDamage(float damageValue)
    {
        float hpData = Mathf.Max(0f, currentHP.Value - damageValue);
        SetHP(hpData);

        //사망 처리
        if (hpData <= 0)
        {
            OnDead?.Invoke();
        }


        damagedTime = Time.time;
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }

        recoveryCoroutine = StartCoroutine(CoRecovery());
    }

    public void Attack(float damage, GameEntity target)
    {
        //RPC
        if (!IsOwner)
        {
            return;
        }

        if (target == null || target.IsInvincible() == true)
        {
            return;
        }

        ServerAttackRpc(damage, target.NetworkObjectId);
    }

    [Rpc(SendTo.Server)]
    public void ServerAttackRpc(float damage, ulong networkObjectID)
    {
        if (IsServer)
        {
            ClientsAttackRpc(damage, networkObjectID);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void ClientsAttackRpc(float damage, ulong networkObjectID)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectID, out var value))
        {
            GameEntity target = value.GetComponent<GameEntity>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }   
        }
    }

    [Rpc(SendTo.Server)]
    public void ServerShootRpc(ulong targetNetworkObjID, string resPath)
    {
        if (IsServer)
        {
            ClientsShootRpc(targetNetworkObjID, resPath);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ClientsShootRpc(ulong targetNetworkObjID, string resPath)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjID, out var value))
        {
            GameEntity target = value.GetComponent<GameEntity>();
            GameObject obj = PoolManager.Instance.SpawnObject(resPath);
            if(obj == null)
            {
                Logger.LogError($"{resPath} is wrong");
                return;
            }

            Projectile projectile = obj.GetComponent<Projectile>();
            if(projectile != null)
            {
                float damage = Formula.CalcDamage(this);
                projectile.transform.position = (projectileTransform == null) ? transform.position : projectileTransform.position;
                projectile.InitProjectile(ProjectileType.Guided, target, 10f, 10f, () =>
                {
                    if (target != null)
                    {
                        target.TakeDamage(damage);
                    }
                });
            }
        }
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

        ServerSetTeamRpc(team);
    }
    public bool IsOpposingTeam(GameEntity other)
    {
        return team.Value != other.GetTeam();
    }

    [Rpc(SendTo.Server)]
    public void ServerSetTeamRpc(Team team)
    {
        if (IsServer)
        {
            this.team.Value = team;
        }
    }

    public IEnumerator CoRecovery()
    {
        while (true)
        {
            if (Time.time - damagedTime < recoveryDelay || currentHP.Value >= maxHP.Value)
            {
                yield return null;
                continue;
            }

            float hpData = Mathf.Min(currentHP.Value + recoveryAmount, maxHP.Value);
            SetHP(hpData);
            yield return new WaitForSeconds(1f);
        }
    }
}
