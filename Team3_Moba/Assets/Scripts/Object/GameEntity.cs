using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


public class GameEntity : NetworkBehaviour 
{ 
    [SerializeField] protected int entityID;
    
    [SerializeReference] protected NetworkVariable<Team> team;
    //  health variable
    [SerializeReference] protected NetworkVariable<bool> isDead; // 초당 회복력
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
    protected Transform markerTransform;

    protected GameObject billboardObj;

    protected virtual void Awake()
    {
        projectileTransform = transform.FindRecursiveChild("ProjectileTransform");
        markerTransform = transform.FindRecursiveChild("Marker");

        team = new NetworkVariable<Team>(Team.None);
        maxHP = new NetworkVariable<float>(0f);
        currentHP = new NetworkVariable<float>(0f);
        isDead = new NetworkVariable<bool>(false);

        maxHP.OnValueChanged += (previous, next) =>
        {
            OnHPChanged?.Invoke(currentHP.Value, next);
        };

        currentHP.OnValueChanged += (previous, next) =>
        {
            OnHPChanged?.Invoke(next, maxHP.Value);
        };
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GameObject billboardPrefab = null;
        Champion champion = this as Champion;
        if (champion != null)
        {
            billboardPrefab = Resources.Load<GameObject>("UI/Billboard/UIChampionBillboard");
        }
        else
        {
            billboardPrefab = Resources.Load<GameObject>("UI/Billboard/UIEntityBillboard");
        }

        if (billboardPrefab != null) 
        {
            billboardObj = Instantiate(billboardPrefab, markerTransform);
            IBillboardActor billboardActor = billboardObj.GetComponent<IBillboardActor>();
            billboardActor.Bind(this);
        }
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

    public void TakeDamage(float damageValue, bool isChampionAttack = false)
    {
        float hpData = Mathf.Max(0f, currentHP.Value - damageValue);
        SetHP(hpData);
        SoundManager.Instance.PlaySFX(3);
        //사망 처리
        if (hpData <= 0)
        {
            //attack판단을 해서 넘겨주긴 해야함.
            Champion champion = this as Champion;
            if (champion != null)
            {
                if (IsServer && isChampionAttack)
                {
                    ServerSetIsDeadRpc(true);
                    MatchManager.Instance.ServerUpdateTeamKillRpc(team.Value);
                }
            }
            OnDead?.Invoke();
        }

        damagedTime = Time.time;
        ResetRecovery();
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
                Champion champion = this as Champion;
                bool isChampionAttack = (champion != null) ? true : false;
                EffectManager.Instance.PlayEffect(1, target.transform.position, new Vector3(1, 1, 1), Quaternion.identity);
                target.TakeDamage(damage, isChampionAttack);
                //해당 이펙트는 없애거나 바꿀듯, 파이어 볼이 맞았을때는 이 이팩트로
                //EffectManager.Instance.PlayEffect(2, target.transform.position, new Vector3(1, 1, 1), Quaternion.identity);
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
            //타워 탑에서 공격 나오는것은 좋음, 이후에 프로젝타일에도 이펙트를 더 달아주면 좋을것 같음
            EffectManager.Instance.PlayEffect(10, projectile.transform, new Vector3(1, 1, 1));

            if (projectile != null)
            {
                float damage = Formula.CalcDamage(this);
                projectile.transform.position = (projectileTransform == null) ? transform.position : projectileTransform.position;
                //이 이펙트도 프로젝타일에 달아줘야됨
                EffectManager.Instance.PlayEffect(9, projectile.transform.transform, new Vector3(1, 1, 1));
                projectile.InitProjectile(ProjectileType.Guided, target, 10f, 10f, () =>
                {
                    if (target != null)
                    {
                        Champion champion = this as Champion;
                        bool isChampionAttack = (champion != null) ? true : false;
                        target.TakeDamage(damage, isChampionAttack);
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

    public void ResetRecovery()
    {
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }
    }

    public IEnumerator CoRecovery()
    {
        while (true)
        {
            if(isDead.Value == true)
            {
                yield break;
            }

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

    [Rpc(SendTo.Server)]
    public void ServerSetIsDeadRpc(bool isDead)
    {
        this.isDead.Value = isDead;
    }
}
