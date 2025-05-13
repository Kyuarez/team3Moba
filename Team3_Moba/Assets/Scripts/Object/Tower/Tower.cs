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

        // Initialize tower-specific data here
        // For example, set the tower's attack damage, range, etc.
        Attack.SetData(data);
    }
    void Update()
    {
        // Update tower logic here
        // For example, check for enemies in range and attack them
    }
}
