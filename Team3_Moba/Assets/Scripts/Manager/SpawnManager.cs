using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//@TK : 아이템 스폰 (네트워크 작업 완료 후 미니언)
public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject expItemObj;

    private List<Vector3> spawnItemPositions;
    private bool isSpawned;
    private int maxSpawnItem = 20;
    private NetworkVariable<int> currentSpawnCount = new NetworkVariable<int>(0);
    private void Start()
    {
        //아이템 스폰 위치 임시 지정
        spawnItemPositions = new List<Vector3>();
        spawnItemPositions.Add(new Vector3(-34f, 3f, -70f));
        spawnItemPositions.Add(new Vector3(-59f, 3f, -39f));
        spawnItemPositions.Add(new Vector3(-84f, 3f, -65f));
        spawnItemPositions.Add(new Vector3(-60f, 3f, -94f));
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (isSpawned == false)
        {
            if (currentSpawnCount.Value >= maxSpawnItem)
            {
                return;
            }
            isSpawned = true;
            StartCoroutine(CoSpawnItem());
        }
    }

    [Rpc(SendTo.Server)]
    public void ServerSpawnRpc(string poolPath, Vector3 spawnPosition)
    {
        if (!IsServer || currentSpawnCount.Value >= maxSpawnItem)
        {
            return;
        }

        GameObject obj = Instantiate(expItemObj, spawnPosition, Quaternion.identity);
        if (obj == null)
        {
            Logger.LogError($"{poolPath} has no NetworkObject!");
            return;
        }
        NetworkObject netObj = obj.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Logger.LogError($"{poolPath} has no NetworkObject!");
            return;
        }
        netObj.Spawn();  // 네트워크에 생성 전파
        currentSpawnCount.Value++;
    }

    int count = 0;
    IEnumerator CoSpawnItem()
    {
        yield return new WaitForSeconds(3f);
        Vector3 positionTemp = spawnItemPositions[UnityEngine.Random.Range(0, 4)];
        float angle = (2f * Mathf.PI / 17) * currentSpawnCount.Value;
        int radius = 4;
        positionTemp.x += Mathf.Cos(angle) * radius;
        positionTemp.z += Mathf.Sin(angle) * radius;
        ServerSpawnRpc("ExpItem" ,positionTemp);

        isSpawned = false;
        count = (count + 1) % 21;
    }
}
