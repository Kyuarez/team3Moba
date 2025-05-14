using System.Collections;
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
    private MatchCameraController matchCamera;

    private Transform playerTransform;
    private Champion playerChampion;
    private List<Vector3> spawnItemPositions;
    private bool isSpawned;
    private int maxSpawnItem = 20;
    private int currentSpawnCount = 0;

    public Transform PlayerTransform => playerTransform;

    private void Start()
    {
        matchCamera = FindAnyObjectByType<MatchCameraController>();
        playerChampion = FindAnyObjectByType<Champion>();
        playerTransform = playerChampion.transform;
        //아이템 스폰 위치 임시 지정
        spawnItemPositions = new List<Vector3>();
        spawnItemPositions.Add(new Vector3(-34f, 3f, -70f));
        spawnItemPositions.Add(new Vector3(-59f, 3f, -39f));
        spawnItemPositions.Add(new Vector3(-84f, 3f, -65f));
        spawnItemPositions.Add(new Vector3(-60f, 3f, -94f));
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

        if(isSpawned == false)
        {
            if(currentSpawnCount >= maxSpawnItem)
            {
                return;
            }

            isSpawned = true;
            StartCoroutine(CoSpawnItem());
        }
    }

    int count = 0;
    IEnumerator CoSpawnItem()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 positionTemp = spawnItemPositions[Random.Range(0,4)];
        float angle = (2f * Mathf.PI / 17) * currentSpawnCount;
        int radius = 4;
        positionTemp.x += Mathf.Cos(angle) * radius;
        positionTemp.z += Mathf.Sin(angle) * radius;
        GameObject item = PoolManager.Instance.SpawnObject("TestItem", positionTemp);

        if(item != null)
        {
            currentSpawnCount++;
        }

        isSpawned = false;
        count = (count + 1) % 21;
    }
}
