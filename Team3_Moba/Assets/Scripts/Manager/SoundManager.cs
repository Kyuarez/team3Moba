using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    public enum SoundType
    {
        BGM,
        SFX,
    }
    public SoundType soundType;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private Coroutine currentBGMCoroutine;

    private Dictionary<int, AudioClip> bgmClipDict = new Dictionary<int, AudioClip>();
    private Dictionary<int, AudioClip> sfxClipDict = new Dictionary<int, AudioClip>();


    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        var soundTableDict = TableManager.Instance.FindAllTableData<SoundTable>();
        foreach (var soundTable in soundTableDict)
        {
            if (soundTable.Value.sound_type == SoundType.BGM.ToString())
            {
                AddBGMDict(soundTable.Value.id, soundTable.Value.path,soundTable.Value.sound_name);
            }
            else if (soundTable.Value.sound_type == SoundType.SFX.ToString())
            {
                AddSFXDict(soundTable.Value.id, soundTable.Value.path, soundTable.Value.sound_name);
            }
        }
    }

    public void AddBGMDict(int soundID, string soundResPath, string soundName)
    {
        Logger.LogWarning($"AddBGM : {soundID}, Sound/{soundResPath}");
        AudioClip clip = Resources.Load<AudioClip>($"Sound/{soundResPath}/{soundName}");
        if (clip == null)
        {
            Logger.LogError($"AudioClip isn't in Resources : {soundResPath}");
            return;
        }

        if (bgmClipDict.ContainsKey(soundID) == true)
        {
            Logger.LogWarning($"Duplicated Sound ID : {soundID}");
            return;
        }

        bgmClipDict.Add(soundID, clip);
    }
    public void AddSFXDict(int soundID, string soundResPath,string soundName)
    {
        Logger.LogWarning($"AddBGM : {soundID}, Sound/{soundResPath}");
        AudioClip clip = Resources.Load<AudioClip>($"Sound/{soundResPath}/{soundName}");
        if (clip == null)
        {
            Logger.LogError($"AudioClip isn't in Resources : {soundResPath}");
            return;
        }

        if (sfxClipDict.ContainsKey(soundID) == true)
        {
            Logger.LogWarning($"Duplicated Sound ID : {soundID}");
            return;
        }

        sfxClipDict.Add(soundID, clip);
    }

    public void PlayBGM(int soundID, float fadeDuration = 1.0f)
    {
        if (bgmClipDict.ContainsKey(soundID) == false)
        {
            return;
        }

        if (currentBGMCoroutine != null)
        {
            StopCoroutine(currentBGMCoroutine);
            currentBGMCoroutine = null;
        }

        StartCoroutine(FadeOutBGMCo(fadeDuration, () =>
        {
            bgmSource.clip = bgmClipDict[soundID];
            bgmSource.Play();
            currentBGMCoroutine = StartCoroutine(FadeInBGMCo(fadeDuration));
        }));
    }

    public void PlaySFX(int soundID)
    {
        if (sfxClipDict.ContainsKey(soundID) == false)
        {
            return;
        }

        sfxSource.PlayOneShot(sfxClipDict[soundID]);
    }
    public void PlaySFX(int soundID, Vector3 position)
    {
        if (sfxClipDict.ContainsKey(soundID) == false)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(sfxClipDict[soundID], position);
    }
    public void PauseBGM()
    {
        bgmSource.Stop();
    }
    public void PauseSFX()
    {
        sfxSource.Stop();
    }
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    private IEnumerator FadeOutBGMCo(float duration, Action onFadeCompleted)
    {
        float startVolume = bgmSource.volume;

        for (float time = 0; time < duration; time++)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        bgmSource.volume = 0;
        onFadeCompleted?.Invoke();
    }
    private IEnumerator FadeInBGMCo(float duration)
    {
        float startVolume = 0;
        bgmSource.volume = 0;

        for (float time = 0; time < duration; time++)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 1f, time / duration);
            yield return null;
        }

        bgmSource.volume = 1f;
    }
}