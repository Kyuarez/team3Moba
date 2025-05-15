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

    // move variable - 보류 태규님과 상의 후 결정

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

    public void Heal(float hpValue) // 힐을 받았을때
    {
        currentHP = Mathf.Min(maxHP, currentHP + hpValue);
    }
    public void TakeDamage(float damageValue) // 데미지를 받았을때
    {
        //Logger.Log("공격 : " + damageValue);
        currentHP = Mathf.Max(0, currentHP - damageValue);

        //사망 처리
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
