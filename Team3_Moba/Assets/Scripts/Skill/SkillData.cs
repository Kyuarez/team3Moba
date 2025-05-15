using UnityEngine;

public enum SkillInputType
{
    Q,
    W,
    E,
    R,
}

public enum SkillExecuteType
{
    Immediately,
    NonTarget,
    SetTarget,
}

public enum SkillActionType
{
    Buff,
    Launch,
}

public class SkillData 
{
    public int SkillID;
    public string PoolPath;
    public string Description;
    public float CoolTime;
    public SkillExecuteType SkillExecuteType;
    public SkillActionType SkillActionType;
    public GameObject SkillFX;
}


