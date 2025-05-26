using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    private BillboardActor actor;
    private Transform cameraTransform;

    public void SetActor(BillboardActor actor)
    {
        actor = this.actor;
    }

    public void Initialize(GameEntity entity)
    {
        cameraTransform = Camera.main.transform;
        //behavior.Initialize(entity);
    }

    private void LateUpdate()
    {
        if (cameraTransform != null)
        {
            transform.forward = cameraTransform.forward;
        }
    }
}
