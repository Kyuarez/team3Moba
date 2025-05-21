using UnityEngine;

public enum MatchCameraState
{
    Free,
    Lock,
    GameOver,
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
    
    private Transform target;

    private void Start()
    {
        MatchManager.Instance.OnGameOver += OnGameOverCamera;
    }

    private void OnEnable()
    {
        cameraState = MatchCameraState.Lock;
        //cameraState = MatchCameraState.Free;
    }

    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }

        switch (cameraState)
        {
            case MatchCameraState.Free:
                FreeMove();
                break;
            case MatchCameraState.Lock:
                LockMove();
                break;
            case MatchCameraState.GameOver:
                GameOverMove();
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
        transform.position = target.position + lockOffset;
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

    private void GameOverMove()
    {
        //좌표 주면 되겠지?
        //transform.Translate()
    }


    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void OnGameOverCamera(Team team)
    {
        cameraState = MatchCameraState.GameOver;
    }

}
