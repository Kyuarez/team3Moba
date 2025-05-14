using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private int itemCount = 20;

    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        InitPoolManager();
    }

    private void InitPoolManager()
    {
        //Item
        GameObject[] itemArr = Resources.LoadAll<GameObject>("Pool");
        if (itemArr != null && itemArr.Length > 0)
        {
            foreach (var item in itemArr)
            {
                GameObject poolObj = new GameObject($"Pool:{item.name}");
                Pool pool = new Pool();
                pool.LoadObject(poolObj.transform, item, 20);
                AddPool(item.name, pool);
            }
        }
    }

    private void AddPool(string path, IPool pool)
    {
        if (poolDict.ContainsKey(path) == true)
        {
            Logger.LogWarning($"Pool's key is duplicated : {path}");
            return;
        }

        poolDict.Add(path, pool);
    }

    public GameObject SpawnObject(string path, Action<GameObject> action = null)
    {
        if (poolDict.ContainsKey(path) == false)
        {
            return null;
        }

        return poolDict[path]?.SpawnObject(Vector3.zero, action);
    }
    public GameObject SpawnObject(string path, Vector2 worldPos, Action<GameObject> action = null)
    {
        if (poolDict.ContainsKey(path) == false)
        {
            return null;
        }

        return poolDict[path]?.SpawnObject(worldPos, action);
    }

    public void DespawnObject(string path, GameObject obj)
    {
        if (poolDict.ContainsKey(path) == false)
        {
            Destroy(obj);
            return;
        }

        poolDict[path].DespawnObject(obj);
    }


    private Dictionary<string, IPool> poolDict = new Dictionary<string, IPool>();

}
