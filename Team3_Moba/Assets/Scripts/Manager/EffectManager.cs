using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static SoundManager;
using UnityEngine.UIElements;

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
            //Logger.Log("����Ʈ  : " + effectList.effect_name);
            AddEffectDict(effectList.id, effectList.path, effectList.effect_name);
        }
    }


    public void AddEffectDict(int effectID, string effectResPath, string effectName)
    {
        Logger.LogWarning($"Effect/{effectResPath}/{effectName}");
        ParticleSystem effect = Resources.Load<ParticleSystem>($"Effect/{effectResPath}/{effectName}");
        if (effect == null)
        {
            Logger.LogError($"ParticleSystem isn't in Resources : {effectResPath}");
            return;
        }
        if (effectDict.ContainsKey(effectID) == true)
        {
            Logger.LogWarning($"Duplicated Effect ID : {effectID}");
            return;
        }
        effectDict.Add(effectID, effect);
    }

    //������ ��ġ ����Ʈ ����
    public void PlayEffect(int id, Vector3 position, Vector3 scale, Quaternion rotation)
    {

        ParticleSystem effect = Instantiate(effectDict[id], position, rotation);
        effect.gameObject.transform.localScale = scale;
        effect.Play();

        Destroy(effect.gameObject, effect.main.duration);
    }

    //�θ� ��ġ ���󰡴� ����Ʈ ����
    public void PlayEffect(int id, Transform parent, Vector3 scale)
    {
        ParticleSystem effect = Instantiate(effectDict[id], parent);
        effect.gameObject.transform.localScale = scale;
        effect.gameObject.transform.rotation = Quaternion.identity;
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration);
    }



}
