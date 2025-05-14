using System.Collections;
using UnityEngine;

public class Formula : MonoBehaviour
{
   
    public static void attack(GameEntity attacker, GameEntity target)
    {
        // 공격자와 타겟의 위치를 비교하여 공격 방향을 계산
        Vector3 direction = (target.gameObject.transform.position - attacker.gameObject.transform.position).normalized;
        // 공격 애니메이션 실행
        //Animator animator = attacker.GetComponent<Animator>();
        //if (animator != null)
        //{
        //    animator.SetTrigger("Attack");
        //}
        // 타겟에게 피해를 입힘
        target.TakeDamage(attacker.GetAttackDamage());
    }
}
