
public abstract class SkillActor
{
    public abstract void Execute(SkillTable data, GameEntity caster, GameEntity target = null);
}
public class LaunchSkillActor : SkillActor
{
    public override void Execute(SkillTable data, GameEntity caster, GameEntity target = null)
    {        
        ProjectileType projectileType = ProjectileType.Guided;
        if(data.excute_type == SkillExecuteType.SetTarget)
        {
            projectileType = ProjectileType.Guided;
        }
        else if(data.excute_type == SkillExecuteType.NoneTarget)
        {
            projectileType = ProjectileType.NoneGuided;
        }

        caster.ServerShootRpc(target.NetworkObjectId, data.skill_name);
    }


}

