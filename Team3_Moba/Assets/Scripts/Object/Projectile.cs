using System;
using UnityEngine;

public enum ProjectileType
{
    NoneGuided,
    Guided, //����ź
}

//@tk : ����ü (�������� �����տ� ������ ����.)
public class Projectile : MonoBehaviour
{
    private ProjectileType projectileType;
    private GameEntity target;
    private float moveSpeed;
    private float duration;
    private Action OnAttack;

    private Vector3 dir;
    private float elapsedTime;

    public void InitProjectile(ProjectileType projectileType, GameEntity target, float moveSpeed, float duration, Action OnAttack = null)
    {
        this.projectileType = projectileType;
        this.target = target;
        this.moveSpeed = moveSpeed;
        this.duration = duration;
        this.OnAttack = OnAttack;

        elapsedTime = 0f;

        if (this.projectileType == ProjectileType.NoneGuided)
        {
            dir = (target.transform.position - transform.position).normalized;
        }
    }

    private void Update()
    {
        if (elapsedTime > duration) 
        {
            DestroyProjectile();
        }

        elapsedTime += Time.deltaTime;

        //����ź�̸� ���� ������Ʈ
        if (projectileType == ProjectileType.Guided)
        {
            dir = (target.transform.position - transform.position).normalized;
        }
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameEntity entity = other.gameObject.GetComponent<GameEntity>();
        if (entity != null)
        {
            if (entity == target)
            {
                OnAttack?.Invoke();
                DestroyProjectile();
            }
        }
    }

    private void DestroyProjectile()
    {
        //TODO : Pool���� üũ�ؼ� ó��
        Destroy(gameObject);
    }
}
