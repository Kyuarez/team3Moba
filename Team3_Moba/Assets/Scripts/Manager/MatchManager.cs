using System.Collections.Generic;
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
                Debug.Log("현재 클릭한 좌표  : " + hit.point);
                Debug.Log("히트된 오브젝트 : " + hit.collider.gameObject.name);
                playerChampion.Move.Move(hit.point);
            }
            else
            {
                // 레이가 맞지 않았을 때도 디버그용으로 DrawRay
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.gray, 1.0f);
            }
        }

        //camera lock - free
        if (Input.GetKeyDown(KeyCode.Space))
        {
            matchCam.SetMatchCameraState(!matchCam.IsLocked);
        }
    }
}
