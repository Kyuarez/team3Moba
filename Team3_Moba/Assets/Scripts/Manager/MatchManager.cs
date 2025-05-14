using UnityEngine;

public enum Team
{
    None,
    Red,
    Blue,
}

public class MatchManager : MonoSingleton<MatchManager>
{
    private MatchCameraController matchCam;

    private Transform playerTrs;
    private Champion playerChampion;

    public Transform PlayerTrs => playerTrs;

    private void Start()
    {
        matchCam = FindAnyObjectByType<MatchCameraController>();
        playerChampion = FindAnyObjectByType<Champion>();
        playerTrs = playerChampion.transform;
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
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 1.0f);
                Logger.Log("���� Ŭ���� ��ǥ  : " + hit.point);
                Logger.Log("��Ʈ�� ������Ʈ : " + hit.collider.gameObject.name);
                playerChampion.Move.Move(hit.point);
            }
        }

        //camera lock - free
        if (Input.GetKeyDown(KeyCode.Space))
        {
            matchCam.SetMatchCameraState(!matchCam.IsLocked);
        }
    }
}
