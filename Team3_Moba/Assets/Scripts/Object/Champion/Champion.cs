using UnityEngine;
using UnityEngine.AI;

public class Champion : GameEntity
{
    public MoveComponent Move;
    public AttackComponent Attack;
    public HealthComponent Health;
    public SkillComponent Skill;

    private void OnEnable()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Move = new MoveComponent(transform, agent);
        Attack = new AttackComponent();
        Health = new HealthComponent();
        Skill = new SkillComponent();
    }

    public override void InitData(EntityData data)
    {
        base.InitData(data);
        Move.SetData(data);
    }
}
