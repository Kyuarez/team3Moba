using System.Collections;
using UnityEngine;

// ������ ���� ��� ���
public class Formula
{

    public static float CalcDamage(GameEntity attacker)
    {
        float damage = 0f;
        return attacker.GetAttackDamage();
    }

}
