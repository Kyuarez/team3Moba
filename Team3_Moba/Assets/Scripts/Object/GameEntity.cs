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
    public void ChangeHP(float hpValue) // ���� �޾�����
    {
        float tmpHp = currentHP + hpValue;

        if (tmpHp < 0)
        {
            currentHP = 0;
        }
        else if (tmpHp > maxHP)
        {
            currentHP = maxHP;
        }
        else  // ��� ó��
        {
            currentHP = tmpHp;
        }
    }
    public void TakeDamage(float damageValue) // �������� �޾�����
    {
        Logger.Log("���� : " + damageValue);

        float tmpHp = currentHP - damageValue;
        if (tmpHp < 0)  // ���ó��
        {
            currentHP = 0;
        }
        else if (tmpHp > maxHP)
        {
            currentHP = maxHP;
        }
        else  // ��� ó��
        {
            currentHP = tmpHp;
        }
    }
    public float GetAttackDamage()
    {
        return attackDamage;
    }
}
