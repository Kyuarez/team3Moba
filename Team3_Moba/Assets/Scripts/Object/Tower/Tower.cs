using UnityEngine;

public class Tower : GameEntity
{
    public AttackComponent Attack;
    public HealthComponent Health;

    private void OnEnable()
    {
        Attack = new AttackComponent();
        Health = new HealthComponent();
    }

    public override void InitData(EntityData data)
    {
        base.InitData(data);

    }
}
