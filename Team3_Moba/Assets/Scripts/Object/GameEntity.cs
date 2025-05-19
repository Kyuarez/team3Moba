using Mono.Cecil;
using System;
using System.Collections;
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

    protected float recoveryDelay = 10f;         // ȸ�� �ڷ�ƾ�� ��µ� �ʿ��� �ð�( �ش� �ð� ��ŭ �������� ���� �ʾƾ��� ȸ��)
    protected float recoveryAmount;              // �ʴ� ȸ����
    private float damagedTime;                   //���������� ������ ���� �ð�
    private Coroutine recoveryCoroutine;

    

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


        damagedTime = Time.time;
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }

        recoveryCoroutine = StartCoroutine(coRecoveryCoroutine());


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
