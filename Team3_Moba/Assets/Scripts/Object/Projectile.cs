using System;
using UnityEngine;

public enum ProjectileType
{
    NoneGuided,
    Guided, //유도탄
}

//@tk : 투사체 (동적으로 프리팹에 연결할 것임.)
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

        //유도탄이면 방향 업데이트
        if (projectileType == ProjectileType.Guided)
        {
            dir = (target.transform.position - transform.position).normalized;
        }
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        CheckCollisionWithTarget();
    }

    private void CheckCollisionWithTarget()
    {
        float hitRadius = 0.5f; // 적절한 충돌 반경 설정
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
        //TODO : Pool인지 체크해서 처리
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
