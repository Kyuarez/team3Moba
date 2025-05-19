using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    private MatchCameraController matchCamera;
    private Champion playerChampion;

    private void Awake()
    {
        matchCamera = FindAnyObjectByType<MatchCameraController>();
        playerChampion = GetComponent<Champion>();

        matchCamera.SetTarget(transform);
    }



    private void Update()
    {

        if (!playerChampion.IsOwner)
        {
            return;
        }

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
                //TODO : 스킬매니저에서 예약된 스킬이 있을 때 발동
                if (SkillManager.Instance.CheckReservationSkill())
                {
                    if (SkillManager.Instance.ExecuteSkill(playerChampion, hit) == true)
                    {
                        return;
                    }
                }

                GameEntity entity = hit.collider.gameObject.GetComponent<GameEntity>();
                if (entity != null)
                {
                    if (playerChampion.IsOpposingTeam(entity))
                    {
                        playerChampion.SetAttackTarget(entity);
                    }
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
            if (playerChampion.PlayerCoolTime.GetRemainingTime(skill.skill_name) != 0)
            {
                return;
            }

            if (skill != null)
            {
                //TODO : 바로 시전인지 타겟 설정인지 
                SkillManager.Instance.SetReservationSkill(skill);
                if (skill.excute_type == SkillExecuteType.Immediately)
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
    }

}
