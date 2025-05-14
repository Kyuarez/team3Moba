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

    // move variable - 보류 태규님과 상의 후 결정


   
       


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
    public void ChangeHP(float hpValue) // 힐을 받았을때
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
        else  // 노멀 처리
        {
            currentHP = tmpHp;
        }
    }
    public void TakeDamage(float damageValue) // 데미지를 받았을때
    {
        Logger.Log("공격 : " + damageValue);

        float tmpHp = currentHP - damageValue;
        if (tmpHp < 0)  // 사망처리
        {
            currentHP = 0;
        }
        else if (tmpHp > maxHP)
        {
            currentHP = maxHP;
        }
        else  // 노멀 처리
        {
            currentHP = tmpHp;
        }
    }
    public float GetAttackDamage()
    {
        return attackDamage;
    }
}
