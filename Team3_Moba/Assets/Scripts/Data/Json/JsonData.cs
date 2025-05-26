using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Wrapper<T>
{
    public List<T> Items;
}

public class EntityData
{
    public int EntityID;
}

public class EntityTable 
{
    public int id;
    public string name;
    public int damage;
    public float attack_range;
    public float attack_cool_time;
    public int hp;
}

public class ChampionTable
{
    public int id;
    public int hp;
    public int mp;
    public float attack;
    public float attack_range;
    public float attack_cool_time;
    public float move_speed;
    public float recovery;
    public int max_level;
    public int current_exp;
    public List<int> skill_list;
}

public class SkillTable
{
    public int id;
    public string skill_name;
    public int damage;
    public string description;
    public float cool_time;
    public SkillExecuteType excute_type;
    public SkillActionType action_type;
    public float duration_time;
    public float duration_damage;
    public float buff_amount;
}

public class LevelTable
{
    // id = level
    public int id;
    public int require_exp;
}


public class SoundTable
{
    public int id;
    public string soundName;
}