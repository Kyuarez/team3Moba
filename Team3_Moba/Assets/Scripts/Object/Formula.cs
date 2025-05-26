using System.Collections;
using UnityEngine;

// ������ ���� ��� ���
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


    //@tk ���� �̷���... ���� ���...
    public static float CalcDamage(GameEntity attacker)
    {
        return attacker.GetAttackDamage();
    }

}
