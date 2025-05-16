using UnityEngine;

public abstract class SkillActor
{
    public abstract void Execute(SkillTable data, GameEntity caster, GameEntity target = null);
}
public class LaunchSkillActor : SkillActor
{
    public override void Execute(SkillTable data, GameEntity caster, GameEntity target = null)
    {
        Projectile projectile = PoolManager.Instance.SpawnObject(data.skill_name).AddComponent<Projectile>();
        projectile.transform.position = caster.transform.position;
        
        ProjectileType projectileType = ProjectileType.Guided;
        if(data.excute_type == SkillExecuteType.SetTarget)
        {
            projectileType = ProjectileType.Guided;
        }
        else if(data.excute_type == SkillExecuteType.NoneTarget)
        {
            projectileType = ProjectileType.NoneGuided;
        }

        projectile.InitProjectile(projectileType, target, 10f, 10f, () => caster.Attack(caster, target));
    }
}

