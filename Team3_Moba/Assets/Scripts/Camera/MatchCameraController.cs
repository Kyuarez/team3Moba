using System.Collections;
using Unity.VisualScripting;
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

    private Transform target;

    public bool IsLocked => cameraState == MatchCameraState.Lock;


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
        if (target == null)
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

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void OnGameOverCamera(Team team)
    {
        target = null;
        //타겟 주기
        if (team == Team.Blue)
        {
            StartCoroutine(CoGameOverCamerea(new Vector3(3.6f, 3f, -6f)));
        }
        else if (team == Team.Red)
        {
            StartCoroutine(CoGameOverCamerea(new Vector3(-122.4f, 3f, -128f)));

        }
    }


    IEnumerator CoGameOverCamerea(Vector3 nexusPosition)
    {
        Vector3 destination = nexusPosition + lockOffset;
        float duration = 2.0f; // 카메라 이동 시간
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, destination, elapsedTime / duration);
            yield return null;
        }

        transform.position = destination;
    }

}
