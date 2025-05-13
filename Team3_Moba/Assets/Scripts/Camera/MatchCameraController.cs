using UnityEngine;

public enum MatchCameraState
{
    Free,
    Lock,
}

/// <summary>
/// @TK : 매치 카메라 동작 로직 (스페이스에 따른 Lock, Free 전환)
/// </summary>
public class MatchCameraController : MonoBehaviour
{
    [SerializeField] private MatchCameraState cameraState;
    [SerializeField] private Vector3 lockOffset;
    
    private float cameraSpeed = 10f;
    private float edgeSize = 20f;
   
    public bool IsLocked => cameraState == MatchCameraState.Lock;
    public Transform Target
    {
        get
        {
            //@tk : 나중에 Netcode로 넘어가면, MatchManager에서 IsOwner 체크해서 전달
            return MatchManager.Instance.PlayerTrs;
        }
    }


    private void OnEnable()
    {
        cameraState = MatchCameraState.Lock;
        //cameraState = MatchCameraState.Free;
    }

    private void LateUpdate()
    {
        switch (cameraState)
        {
            case MatchCameraState.Free:
                FreeMove();
                break;
            case MatchCameraState.Lock:
                LockMove();
                break;
            default:
                break;
        }
    }

    public void SetMatchCameraState(bool locked)
    {
        cameraState = (locked == true) ? MatchCameraState.Lock : MatchCameraState.Free;
    }

    private void LockMove()
    {
        transform.position = Target.position + lockOffset;
    }

    private void FreeMove()
    {
        Vector3 move = Vector3.zero;

        //@tk : Dir setting
        if (Input.mousePosition.x >= Screen.width - edgeSize)
            move.x += 1;
        if (Input.mousePosition.x <= edgeSize)
            move.x -= 1;
        if (Input.mousePosition.y >= Screen.height - edgeSize)
            move.z += 1;
        if (Input.mousePosition.y <= edgeSize)
            move.z -= 1;

        transform.Translate(move.normalized * cameraSpeed * Time.deltaTime, Space.World);
    }
}
