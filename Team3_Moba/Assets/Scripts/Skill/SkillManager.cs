using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//��ų �Ŵ����� ����
//�����... ��ų ������ ������, ��ų �������� �������� ���� Ŭ�������� �ٿ��ִ� ����
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

    //��� ����
    public bool ExecuteSkill(GameEntity caster) 
    {
        if (reservationSkill == null) 
        {
            return false;
        }

        return Execute(caster);
    }
    //��ǥ, Ÿ��
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

        Logger.LogWarning("SkillActionType�� �ش��ϴ� SKillActor�� ����");
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
        //TODO �ε������� ���� �� ����
        reservationSkill = null;
    }

    public bool CheckReservationSkill()
    {
        return reservationSkill != null;
    }
    
}

