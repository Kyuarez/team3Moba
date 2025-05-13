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
    public Transform PlayerTrs => playerTrs;

    private void Start()
    {
        matchCam = FindAnyObjectByType<MatchCameraController>();
    }

    private void Update()
    {
        //InputManager
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                //TODO
            }
        }

        //camera lock - free
        if (Input.GetKeyDown(KeyCode.Space))
        {
            matchCam.SetMatchCameraState(!matchCam.IsLocked);
        }
    }
}
