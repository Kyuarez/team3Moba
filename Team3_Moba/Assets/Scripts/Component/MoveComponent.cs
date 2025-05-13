using UnityEngine;
using UnityEngine.AI;

public class MoveComponent 
{
    private Transform ownerTransform;
    private NavMeshAgent agent;
    private float moveSpeed;
    private float rotateSpeed;
    private float rotateVelocity;

    public MoveComponent(Transform owner, NavMeshAgent agent)
    {
        this.ownerTransform = owner;
        this.agent = agent;
    }

    public void Move(Vector3 destination)
    {
        //Move
        agent.SetDestination(destination);
        agent.stoppingDistance = 0;

        //Rotate
        Quaternion rotationToLookat = Quaternion.LookRotation(destination - ownerTransform.position);
        float rotationY = Mathf.SmoothDampAngle(ownerTransform.eulerAngles.y,
            rotationToLookat.eulerAngles.y, ref rotateVelocity, rotateSpeed * 5f * Time.deltaTime);

        ownerTransform.eulerAngles = new Vector3(0f, rotationY, 0f);
    }
}
