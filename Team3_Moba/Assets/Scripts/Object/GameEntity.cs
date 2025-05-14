using UnityEngine;

public class GameEntity : MonoBehaviour
{
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


    public virtual void InitData(EntityData data)
    {
        this.attackDamage = 15f;
        this.attackRange = 10f;
        this.attackCoolTime = 3f;
        this.attackDelay = 1f;

        this.maxHP = 50;
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
        Logger.Log("���� : " + damageValue);
        currentHP = Mathf.Max(0, currentHP - damageValue);
    }

    public void Attack(GameEntity attacker, GameEntity target)
    {
        Vector3 direction = (target.gameObject.transform.position - attacker.gameObject.transform.position).normalized;
        target.TakeDamage(Formula.CalcDamage(attacker));
    }


}
