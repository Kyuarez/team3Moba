using UnityEngine;
using UnityEngine.AI;

//@TK : TestÄÚµå
public class HeroMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    private float rotateSpeedMovement = 0.05f;
    private float rotateVelocity;
    private float animSmoothTime = 0.1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        anim.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed, animSmoothTime, Time.deltaTime);
    }

    public void SetDestination(Vector3 hitPoint)
    {
        //Move
        agent.SetDestination(hitPoint);
        agent.stoppingDistance = 0;

        //Rotate
        Quaternion rotationToLookat = Quaternion.LookRotation(hitPoint - transform.position);
        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookat.eulerAngles.y, ref rotateVelocity, rotateSpeedMovement * 5f * Time.deltaTime);

        transform.eulerAngles = new Vector3(0f, rotationY, 0f);
    }
}
