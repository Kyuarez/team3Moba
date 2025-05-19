using Mono.Cecil;
using System;
using UnityEditor.Experimental.GraphView;
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
    //@TK : ���� MVC ���Ͽ� �°� Stat�����ϴ� ���� Ŭ���� �ʿ�
    public event Action<float, float> OnHPChanged;
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
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(float damageValue) // �������� �޾�����
    {
        //Logger.Log("���� : " + damageValue);
        currentHP = Mathf.Max(0, currentHP - damageValue);
        OnHPChanged?.Invoke(currentHP, maxHP);

        //��� ó��
        if (currentHP <= 0)
        {
            OnDead?.Invoke();
        }

    }

    public void Attack(float damage, GameEntity target)
    {
        if(target == null)
        {
            return;
        }

        target.TakeDamage(damage);
    }

    public Team GetTeam()
    {
        return team;
    }

    public bool IsOpposingTeam(GameEntity other)
    {
        return team != other.GetTeam();
    }
}
