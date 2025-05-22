using System.Collections.Generic;
using UnityEngine;

//��ų �Ŵ����� ����
//�����... ��ų ������ ������, ��ų �������� �������� ���� Ŭ�������� �ٿ��ִ� ����
public class SkillManager : MonoSingleton<SkillManager>
{
    private SkillTable reservationSkill;
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

        Champion chapion = caster as Champion;
        if(chapion != null) 
        {
            chapion.PlayerCoolTime.SetCoolTime(reservationSkill.skill_name, reservationSkill.cool_time);
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

        if (reservationSkill.excute_type == SkillExecuteType.SetTarget)
        {
            GameEntity target = hit.collider.gameObject.GetComponent<GameEntity>();
            if(target != null && caster.GetTeam() != target.GetTeam())
            {
                return Execute(caster, target);
            }
        }
        else if(reservationSkill.excute_type == SkillExecuteType.NoneTarget)
        {
            return Execute(caster);
        }

        ResetReservationSkill();
        return false;
    }

    private bool Execute(GameEntity caster, GameEntity target = null)
    {
        if(skillActorDict.TryGetValue(reservationSkill.action_type, out var skillActor))
        {
            Logger.Log($"{reservationSkill.skill_name} is act!");
            skillActor.Execute(reservationSkill, caster, target);
            Champion chapion = caster as Champion;
            if (chapion != null)
            {
                chapion.PlayerCoolTime.SetCoolTime(reservationSkill.skill_name, reservationSkill.cool_time);
            }
            ResetReservationSkill();
            return true;
        }

        Logger.LogWarning("SkillActionType�� �ش��ϴ� SKillActor�� ����");
        return false;
    }

    public void SetReservationSkill(SkillTable skill)
    {
        if (reservationSkill != null) 
        {
            //TODO
            ResetReservationSkill();
        }

        Logger.Log($"{skill.skill_name} is reservation!");
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