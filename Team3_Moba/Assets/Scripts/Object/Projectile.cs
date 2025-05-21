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

    private void ResetProjectile()
    {
        projectileType = ProjectileType.NoneGuided;
        target = null;
        moveSpeed = 0f;
        duration = 0f;
        OnAttack = null;
    }

    private void Update()
    {
        if (elapsedTime > duration) 
        {
            DestroyProjectile();
            return;
        }

        if(target == null)
        {
            DestroyProjectile();
            return;
        }

        elapsedTime += Time.deltaTime;

        //����ź�̸� ���� ������Ʈ
        if (projectileType == ProjectileType.Guided)
        {
            dir = (target.transform.position - transform.position).normalized;
        }
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        CheckCollisionWithTarget();
    }

    private void CheckCollisionWithTarget()
    {
        float hitRadius = 0.5f; // ������ �浹 �ݰ� ����
        Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
        foreach (var hit in hits)
        {
            GameEntity entity = hit.GetComponent<GameEntity>();
            if (entity != null && entity == target)
            {
                OnAttack?.Invoke();
                DestroyProjectile();
                break;
            }
        }
    }

    private void DestroyProjectile()
    {
        //TODO : Pool���� üũ�ؼ� ó��
        ResetProjectile();
        string path = gameObject.name.Replace("(Clone)", "");
        if (PoolManager.Instance.IsContainPool(path))
        {
            PoolManager.Instance.DespawnObject(path, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
