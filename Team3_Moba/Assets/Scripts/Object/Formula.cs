using System.Collections;
using UnityEngine;

// 레벨에 따른 계산 기능
public class Formula
{
    public static int CalcHP(int defaultHP, int currentLevel)
    {
       return defaultHP + currentLevel * 10;
    }
    public static float CalcAttack(float defaultAttck, int currentLevel)
    {
        return defaultAttck + (currentLevel - 1) * 10f;
    }
    public static float CalcRecovery(float defaultRecovery, int currentLevel)
    {
        return defaultRecovery + (currentLevel - 1);
    }


    //@tk 버프 이런거... 같이 계산...
    public static float CalcDamage(GameEntity attacker)
    {
        return attacker.GetAttackDamage();
    }

}
