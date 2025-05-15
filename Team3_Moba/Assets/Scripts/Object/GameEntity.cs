using Mono.Cecil;
using System;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    [SerializeField]
    protected Team team;

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


    public virtual void InitData(EntityData data)
    {
        this.attackDamage = 15f;
        this.attackRange = 10f;
        this.attackCoolTime = 3f;
        this.attackDelay = 1f;

        this.maxHP = 100;
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
