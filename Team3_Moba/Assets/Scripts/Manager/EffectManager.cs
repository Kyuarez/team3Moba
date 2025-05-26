using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static SoundManager;

public class EffectManager : MonoSingleton<EffectManager>
{

    //����Ʈ ��ųʸ� �����
    private Dictionary<int, ParticleSystem> effectDict = new Dictionary<int, ParticleSystem>();

    //����Ʈ�� ��ųʸ��� �ִ� ���� ����

    //

    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        var EffectTableDict = TableManager.Instance.FindAllTableData<EffectTable>();
        foreach (var EffectTable in EffectTableDict)
        {
            var effectList = EffectTable.Value;
            Logger.Log($"dlfma  :  {effectList.name}");
        }
    }

    void Update()
    {
        
    }
}
