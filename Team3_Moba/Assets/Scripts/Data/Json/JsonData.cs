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