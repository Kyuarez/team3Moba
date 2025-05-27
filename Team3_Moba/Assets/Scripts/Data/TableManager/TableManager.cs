using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

//@tk : 시작할 때 로드할 데이터 관리 매니저
public partial class TableManager 
{
    #region Singletone
    private static TableManager instance;
    public static TableManager Instance
    {
        get { return instance; }
    }
    
    
    #endregion

    //@tk 게임 시작하고, 처음 호출(일단 게임씬 매니저에서 호출하되 나중에 타이틀로 옮기기) : 25.04.23
    public void OnLoadGameAction()
    {
        instance = this;

        LoadAllTables();
    }


    //@tk 일단 타이틀에서 작업하되 
    private void LoadAllTables()
    {
        LoadTable<EntityTable>("EntityTable", out entityTable, x => x.id);
        LoadTable<ChampionTable>("ChampionTable", out championTable, x => x.id);
        LoadTable<SkillTable>("SkillTable", out skillTable, x => x.id);
        LoadTable<LevelTable>("LevelTable", out levelTable, x => x.id);
        LoadTable<SoundTable>("SoundTable", out soundTable, x => x.id);
        LoadTable<EffectTable>("EffectTable", out effectTable, x => x.id);
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
        Logger.Log(tableName.ToString());
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
    public Dictionary<int, T> FindAllTableData<T>() where T : class
    {
        if (tableMap.TryGetValue(typeof(T), out var obj))
        {
            var typedDict = obj as Dictionary<int, T>;
            if (typedDict != null)
                return typedDict;
        }
        Logger.LogError($"[TableManager] No data found for type {typeof(T).Name}");
        return new Dictionary<int, T>();
    }
    #region DataTable
    private Dictionary<Type, object> tableMap = new Dictionary<Type, object>();

    private Dictionary<int, EntityTable> entityTable = new Dictionary<int, EntityTable>();
    private Dictionary<int, ChampionTable> championTable = new Dictionary<int, ChampionTable>();
    private Dictionary<int, SkillTable> skillTable = new Dictionary<int, SkillTable>();
    private Dictionary<int, LevelTable> levelTable = new Dictionary<int , LevelTable>();
    private Dictionary<int, SoundTable> soundTable = new Dictionary<int, SoundTable>();
    private Dictionary<int, EffectTable> effectTable = new Dictionary<int , EffectTable>();
    #endregion
}
