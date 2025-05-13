using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

//@tk : ������ �� �ε��� ������ ���� �Ŵ���
public partial class TableManager 
{
    #region Singletone
    private static TableManager instance;
    public static TableManager Instance
    {
        get { return instance; }
    }
    
    
    #endregion

    //@tk ���� �����ϰ�, ó�� ȣ��(�ϴ� ���Ӿ� �Ŵ������� ȣ���ϵ� ���߿� Ÿ��Ʋ�� �ű��) : 25.04.23
    public void OnLoadGameAction()
    {
        instance = this;

        LoadAllTables();
    }


    //@tk �ϴ� Ÿ��Ʋ���� �۾��ϵ� 
    private void LoadAllTables()
    {
        //LoadTable<JsonIngameObject>("JsonIngameObject", out ingameObjectDict, x => x.ObjectID);
       
    }

    private void LoadTable<T>(string tableName, out Dictionary<int, T> outDict, System.Func<T, int> keySelector)
    {
        string path = Application.streamingAssetsPath + $"/{tableName}.json";
        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            outDict = new Dictionary<int, T>();
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        string jsonText = File.ReadAllText(path);
#else
        UnityWebRequest www = UnityWebRequest.Get(path);
        www.SendWebRequest();
        while (!www.isDone) { }
        jsonText = www.downloadHandler.text;
#endif

        Dictionary<string, List<T>> parsed = JsonConvert.DeserializeObject<Dictionary<string, List<T>>>(jsonText);
        List<T> items = parsed[tableName];

        outDict = new Dictionary<int, T>();
        foreach (var item in items)
        {
            outDict[keySelector(item)] = item;
        }
        RegisterTable<T>(outDict);
    }

    private void RegisterTable<T>(Dictionary<int, T> table)
    {
        tableMap[typeof(T)] = table;
    }

    public T FindTableData<T>(int id) where T : class
    {
        if (tableMap.TryGetValue(typeof(T), out var obj))
        {
            var typedDict = obj as Dictionary<int, T>;
            if (typedDict != null && typedDict.TryGetValue(id, out var result))
                return result;
        }

        Debug.LogError($"[TableManager] No data found for type {typeof(T).Name} and ID {id}");
        return null;
    }

    #region DataTable
    private Dictionary<Type, object> tableMap = new Dictionary<Type, object>();

    //private Dictionary<int, JsonIngameObject> ingameObjectDict = new Dictionary<int, JsonIngameObject>();

    #endregion
}
