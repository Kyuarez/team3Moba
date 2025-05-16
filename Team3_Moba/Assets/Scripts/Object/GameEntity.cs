using Mono.Cecil;
using System;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    [SerializeField] protected Team team;
    [SerializeField] protected int entityID;

    //  attack variable
    protected float attackDamage;
    protected float attackRange;
    protected float attackCoolTime;
    protected float attackDelay;

    //  health variable
    protected float maxHP;
    protected float currentHP;

    // move variable - ���� �±Դ԰� ���� �� ����

    public event Action OnDead;

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
    }

    public float GetHP()
    {
        return currentHP;
    }
    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void Heal(float hpValue) // ���� �޾�����
    {
        currentHP = Mathf.Min(maxHP, currentHP + hpValue);
    }
    public void TakeDamage(float damageValue) // �������� �޾�����
    {
        //Logger.Log("���� : " + damageValue);
        currentHP = Mathf.Max(0, currentHP - damageValue);

        //��� ó��
        if (currentHP <= 0)
        {
            OnDead?.Invoke();
        }

    }

    public void Attack(GameEntity attacker, GameEntity target)
    {
        if (attacker.team != target.team)
        {
            Vector3 direction = (target.gameObject.transform.position - attacker.gameObject.transform.position).normalized;
            target.TakeDamage(Formula.CalcDamage(attacker));
        }

    }
    public Team GetTeam()
    {
        return team;
    }
}
