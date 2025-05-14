using UnityEngine;
using UnityEngine.AI;

public class Champion : GameEntity
{
    private Animator championAnimator;
    private NavMeshAgent agent;

    private float moveSpeed;
    private float rotateSpeed;
    private float rotateVelocity;

    private float animSmoothTime = 0.1f;

    public AttackComponent Attack;
    public HealthComponent Health;
    public SkillComponent Skill;

    private void OnEnable()
    {
        championAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        Attack = new AttackComponent();
        Health = new HealthComponent();
        Skill = new SkillComponent();

        #region Test
        //@tk : agent ¼¼ÆÃ test
        moveSpeed = 10f;
        agent.speed = moveSpeed;
        agent.angularSpeed = 10000f;
        agent.acceleration = 10000f;
        #endregion
    }

    private void Update()
    {
        championAnimator.SetFloat("MoveFactor", agent.velocity.magnitude / agent.speed, animSmoothTime, Time.deltaTime);
    }

    public override void InitData(EntityData data)
    {
        base.InitData(data);
        
    }

    public void Move(Vector3 destination)
    {
        //Move
        agent.SetDestination(destination);
        agent.stoppingDistance = 0;

        //Rotate
        Quaternion rotationToLookat = Quaternion.LookRotation(destination - transform.position);
        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookat.eulerAngles.y, ref rotateVelocity, rotateSpeed * 5f * Time.deltaTime);

        transform.eulerAngles = new Vector3(0f, rotationY, 0f);
    }
}
