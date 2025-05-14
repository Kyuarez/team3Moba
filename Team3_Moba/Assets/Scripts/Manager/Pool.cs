using System.Collections.Generic;
using System;
using UnityEngine;

public interface IPool
{
    public Transform Parent { get; set; }

    Queue<GameObject> pool { get; set; }

    public void LoadObject(Transform parent, GameObject prefab, int maxCount);
    public GameObject SpawnObject(Vector3 worldPos, Action<GameObject> action = null);
    public void DespawnObject(GameObject obj, Action<GameObject> action = null);
}

public class Pool : IPool
{
    public Transform Parent
    {
        get;
        set; //@tk (25.04.09 : �̰� private�� �����ϴ� ��� ã�ƾ� ��.)
    }
    public Queue<GameObject> pool
    {
        get;
        set;
    }
    public void LoadObject(Transform parent, GameObject prefab, int maxCount)
    {
        //Ǯ ����
        Parent = parent;
        pool = new Queue<GameObject>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            DespawnObject(obj);
        }
    }

    public GameObject SpawnObject(Vector3 worldPos, Action<GameObject> action = null)
    {
        if(pool.Count == 0)
        {
            return null;
        }
        var obj = pool.Dequeue();
        if (obj != null)
        {
            obj.transform.SetParent(null);
            obj.transform.position = worldPos;
            obj.gameObject.SetActive(true);
            action?.Invoke(obj);
            return obj;
        }

        return null;
    }

    public void DespawnObject(GameObject obj, Action<GameObject> action = null)
    {
        pool.Enqueue(obj);
        obj.SetActive(false);
        obj.transform.SetParent(Parent);
        obj.transform.position = Vector2.zero;
        obj.transform.rotation = Quaternion.identity;
        action?.Invoke(obj);
    }
}
