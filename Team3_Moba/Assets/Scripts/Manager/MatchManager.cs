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

    private Vector3 spawnRedTeamPosition = new Vector3(19f, 6f, 5f);
    private Vector3 spawnBlueTeamPosition = new Vector3(-135f, 6f, -140f);

    public Transform PlayerTransform => playerTransform;

    protected override void Awake()
    {
        base.Awake();
        TableManager table = new TableManager();
        table.OnLoadGameAction();
    }
    private void Start()
    {
        matchCamera = FindAnyObjectByType<MatchCameraController>();
        playerChampion = FindAnyObjectByType<Champion>();
        playerTransform = playerChampion.transform;
        //������ ���� ��ġ �ӽ� ����
        spawnItemPositions = new List<Vector3>();
        spawnItemPositions.Add(new Vector3(-34f, 3f, -70f));
        spawnItemPositions.Add(new Vector3(-59f, 3f, -39f));
        spawnItemPositions.Add(new Vector3(-84f, 3f, -65f));
        spawnItemPositions.Add(new Vector3(-60f, 3f, -94f));

        playerChampion.OnDeadComplete += OnChampionDeadComplete;
    }

    private void Update()
    {
        //InputManager
        if (Input.GetMouseButtonDown(1))
        {
            if (playerChampion.GetHP() == 0)
            {
                return;
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //TODO : ��ų�Ŵ������� ����� ��ų�� ���� �� �ߵ�
                if (SkillManager.Instance.CheckReservationSkill())
                {
                    if(SkillManager.Instance.ExecuteSkill(playerChampion, hit) == true)
                    {
                        return;
                    }
                }

                GameEntity entity = hit.collider.gameObject.GetComponent<GameEntity>();
                if (entity != null)
                {
                    playerChampion.SetAttackTarget(entity);
                }
                else
                {
                    playerChampion.ResetAttackTarget();
                    playerChampion.Move(hit.point);
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (playerChampion.GetHP() == 0)
            {
                return;
            }

            SkillTable skill = playerChampion.GetSkillData(SkillInputType.Q);
            //TODO : ��Ÿ�� üũ�ϴ� ���� �ʿ�
            if(skill != null)
            {
                //TODO : �ٷ� �������� Ÿ�� �������� 
                SkillManager.Instance.SetReservationSkill(skill);
                if(skill.excute_type == SkillExecuteType.Immediately)
                {
                    SkillManager.Instance.ExecuteSkill(playerChampion);
                }
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

    public void OnChampionDeadComplete()
    {
        if (playerChampion.GetTeam() == Team.Red)
        {
            playerChampion.transform.position = spawnRedTeamPosition;
        }
        else if(playerChampion.GetTeam() == Team.Blue)
        {
            playerChampion.transform.position = spawnBlueTeamPosition;
        }

        
    }
}
