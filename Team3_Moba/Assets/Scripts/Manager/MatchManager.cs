using UnityEngine;

public class MatchManager : MonoSingleton<MatchManager>
{
    private MatchCameraController matchCam;
    private HeroMove heroMove;

    private Transform playerTrs;
    public Transform PlayerTrs => playerTrs;

    private void Start()
    {
        matchCam = FindAnyObjectByType<MatchCameraController>();
        heroMove = FindAnyObjectByType<HeroMove>(); //@tk 당연히 나중엔 맵 생성하고 받는 방법으로
        playerTrs = heroMove.transform;
    }

    private void Update()
    {
        //InputManager
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                heroMove?.SetDestination(hit.point);
            }
        }

        //camera lock - free
        if (Input.GetKeyDown(KeyCode.Space))
        {
            matchCam.SetMatchCameraState(!matchCam.IsLocked);
        }
    }
}
