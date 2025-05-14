using System.Collections;
using UnityEngine;

// 레벨에 따른 계산 기능
public class Formula
{

    public static float CalcDamage(GameEntity attacker)
    {
        float damage = 0f;
        return attacker.GetAttackDamage();
    }

}
