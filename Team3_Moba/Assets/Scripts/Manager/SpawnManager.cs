using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//@TK : ������ ���� (��Ʈ��ũ �۾� �Ϸ� �� �̴Ͼ�)
public class SpawnManager : NetworkBehaviour
{
    private List<Vector3> spawnItemPositions;
    private bool isSpawned;
    private int maxSpawnItem = 20;
    private int currentSpawnCount = 0;
    private void Start()
    {
        //������ ���� ��ġ �ӽ� ����
        spawnItemPositions = new List<Vector3>();
        spawnItemPositions.Add(new Vector3(-34f, 3f, -70f));
        spawnItemPositions.Add(new Vector3(-59f, 3f, -39f));
        spawnItemPositions.Add(new Vector3(-84f, 3f, -65f));
        spawnItemPositions.Add(new Vector3(-60f, 3f, -94f));

    }

    private void Update()
    {
        if (isSpawned == false)
        {
            if (currentSpawnCount >= maxSpawnItem)
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
        yield return new WaitForSeconds(3f);
        Vector3 positionTemp = spawnItemPositions[UnityEngine.Random.Range(0, 4)];
        float angle = (2f * Mathf.PI / 17) * currentSpawnCount;
        int radius = 4;
        positionTemp.x += Mathf.Cos(angle) * radius;
        positionTemp.z += Mathf.Sin(angle) * radius;
        GameObject item = PoolManager.Instance.SpawnObject("TestItem", positionTemp);
        ExpItem expItem = item.GetComponent<ExpItem>();
        if (expItem != null)
        {
            //TODO : Ŭ���̾�Ʈ ���̵�� �����ؼ� EXP �þ��
            //expItem.Initialize("TestItem", 3, playerChampion.OnGetExpItem);
        }

        if (item != null)
        {
            currentSpawnCount++;
        }

        isSpawned = false;
        count = (count + 1) % 21;
    }

    public void DecreaseExpItemCount()
    {
        Mathf.Min(0, --currentSpawnCount);
    }
}
