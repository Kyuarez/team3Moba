using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//스킬 매니저의 역할
//모듈적... 스킬 데이터 받으면, 스킬 데이터의 정보값에 따른 클래스들을 붙여주는 역할
public class SkillManager : MonoSingleton<SkillManager>
{
    private SkillData reservationSkill;
    private Dictionary<SkillActionType, SkillActor> skillActorDict = new Dictionary<SkillActionType, SkillActor>();

    protected override void Awake()
    {
        base.Awake();

        skillActorDict.Clear();
        skillActorDict.Add(SkillActionType.Launch, new LaunchSkillActor());
    }

    //즉시 공격
    public bool ExecuteSkill(GameEntity caster) 
    {
        if (reservationSkill == null) 
        {
            return false;
        }

        return Execute(caster);
    }
    //좌표, 타겟
    public bool ExecuteSkill(GameEntity caster, RaycastHit hit)
    {
        if (reservationSkill == null)
        {
            return false;
        }

        if (reservationSkill.SkillExecuteType == SkillExecuteType.SetTarget)
        {
            GameEntity target = hit.collider.gameObject.GetComponent<GameEntity>();
            if(target != null)
            {
                return Execute(caster, target);
            }
        }
        else if(reservationSkill.SkillExecuteType == SkillExecuteType.NonTarget)
        {
            return Execute(caster);
        }

        ResetReservationSkill();
        return false;
    }

    private bool Execute(GameEntity caster, GameEntity target = null)
    {
        if(skillActorDict.TryGetValue(reservationSkill.SkillActionType, out var skillActor))
        {
            Logger.Log($"{reservationSkill.SkillID} is act!");
            skillActor.Execute(reservationSkill, caster, target);
            ResetReservationSkill();
            return true;
        }

        Logger.LogWarning("SkillActionType에 해당하는 SKillActor가 없다");
        return false;
    }

    public void SetReservationSkill(SkillData skill)
    {
        if (reservationSkill != null) 
        {
            //TODO
            ResetReservationSkill();
        }

        Logger.Log($"{skill.SkillID} is reservation!");
        reservationSkill = skill;
    }
    public void ResetReservationSkill()
    {
        //TODO 인디케이터 끄기 등 뭔가
        reservationSkill = null;
    }

    public bool CheckReservationSkill()
    {
        return reservationSkill != null;
    }
    
}

