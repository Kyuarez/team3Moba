using System.Collections;
using UnityEngine;

public class Formula : MonoBehaviour
{
   
    public static void attack(GameEntity attacker, GameEntity target)
    {
        // �����ڿ� Ÿ���� ��ġ�� ���Ͽ� ���� ������ ���
        Vector3 direction = (target.gameObject.transform.position - attacker.gameObject.transform.position).normalized;
        // ���� �ִϸ��̼� ����
        //Animator animator = attacker.GetComponent<Animator>();
        //if (animator != null)
        //{
        //    animator.SetTrigger("Attack");
        //}
        // Ÿ�ٿ��� ���ظ� ����
        target.TakeDamage(attacker.GetAttackDamage());
    }
}
