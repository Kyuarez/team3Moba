using System;
using System.Collections.Generic;
using UnityEngine;
public class CoolTimeData
{
    public string key;
    public float startTime;
    public float endTime;
    public float duration;

    public float RemainingTime => Mathf.Max(0f, startTime + duration - Time.time);
    public bool IsOver => Time.time >= endTime;

    public CoolTimeData(string key, float duration)
    {
        this.key = key;
        this.startTime = Time.time;
        this.endTime = Time.time + duration;
        this.duration = duration;
    }
}

//@TK : NetCode 연동 시, 서버에서만 돌리게 하기
public class CoolTimeManager 
{
    //key - 종료 시간
    private Dictionary<string, CoolTimeData> coolTimeDict = new Dictionary<string, CoolTimeData>();
    public event Action<CoolTimeData> OnCoolTimeUpdate;

    public void SetCoolTime(string key, float duration)
    {
        //@TK : TODO; NetCode 연동 시 
        CoolTimeData coolTimeData = new CoolTimeData(key, duration);
        coolTimeDict[key] = coolTimeData;
    }
    public void Update()
    {
        List<string> ended = new List<string>();
        foreach (var kvp in coolTimeDict)
        {
            CoolTimeData data = kvp.Value;
            OnCoolTimeUpdate?.Invoke(data);
            if (data.IsOver == true) 
            {
                ended.Add(data.key);
            }
        }

        foreach (string key in ended)
        {
            coolTimeDict.Remove(key);
        }
    }

    public float GetRemainingTime(string key)
    {
        if(coolTimeDict.ContainsKey(key) == false)
        {
            return 0;
        }

        return coolTimeDict[key].RemainingTime;
    }
}
