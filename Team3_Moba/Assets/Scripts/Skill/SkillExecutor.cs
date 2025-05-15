using UnityEngine;

public abstract class SkillActor
{
    public abstract void Execute(SkillData data, GameEntity caster, GameEntity target = null);
}
public class LaunchSkillActor : SkillActor
{
    public override void Execute(SkillData data, GameEntity caster, GameEntity target = null)
    {
        Projectile projectile = PoolManager.Instance.SpawnObject(data.PoolPath).AddComponent<Projectile>();
        projectile.transform.position = caster.transform.position;
        
        ProjectileType projectileType = ProjectileType.Guided;
        if(data.SkillExecuteType == SkillExecuteType.SetTarget)
        {
            projectileType = ProjectileType.Guided;
        }
        else if(data.SkillExecuteType == SkillExecuteType.NonTarget)
        {
            projectileType = ProjectileType.NoneGuided;
        }

        projectile.InitProjectile(projectileType, target, 10f, 10f, () => caster.Attack(caster, target));
    }
}

