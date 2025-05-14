using UnityEngine;

public enum Team
{
    None,
    Red,
    Blue,
}

public class MatchManager : MonoSingleton<MatchManager>
{
    private MatchCameraController matchCamera;

    private Transform playerTransform;
    private Champion playerChampion;

    public Transform PlayerTransform => playerTransform;

    private void Start()
    {
        matchCamera = FindAnyObjectByType<MatchCameraController>();
        playerChampion = FindAnyObjectByType<Champion>();
        playerTransform = playerChampion.transform;
    }

    private void Update()
    {
        //InputManager
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //if()

                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 1.0f);
                Logger.Log("현재 클릭한 좌표  : " + hit.point);
                Logger.Log("히트된 오브젝트 : " + hit.collider.gameObject.name);
                playerChampion.Move(hit.point);
            }
        }

        //camera lock - free
        if (Input.GetKeyDown(KeyCode.Space))
        {
            matchCamera.SetMatchCameraState(!matchCamera.IsLocked);
        }
    }
}
