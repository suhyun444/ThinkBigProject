using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    [SerializeField] SoundData soundSetting;

    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioSource bgmAudioSource = null;
    private Dictionary<Sound, AudioClip> soundEffectDictionary;
    private float sfxVolume = 1.0f;
    private float bgmVolume = 1.0f;
    private bool isStopped = false;
    private void Awake()
    {
        InitSingleTon(this);
        soundEffectDictionary = new Dictionary<Sound, AudioClip>();
        InitSoundDictionary();
        audioSource.volume = SaveManager.Instance.GetVolumeData();
        sfxVolume = audioSource.volume;
        bgmAudioSource.volume = 0.3f * SaveManager.Instance.GetVolumeData();
        bgmVolume = bgmAudioSource.volume;
    }
    private void InitSoundDictionary()
    {
        int count = soundSetting.count;
        for(int i = 0; i < count; i++)
        {
            soundEffectDictionary.Add(soundSetting.nameList[i], soundSetting.clipList[i]);
        }
    }

    public void PlaySoundEffect(Sound name)
    {
        if (!soundEffectDictionary.ContainsKey(name))
        {
            Debug.LogError($"{name} is not available");
            return;
        }
        RandomPitch();
        audioSource.PlayOneShot(soundEffectDictionary[name]);
    }
    public void PlayBGM(Sound name)
    {
        if (!soundEffectDictionary.ContainsKey(name))
        {
            Debug.LogError($"{name} is not available");
            return;
        }
        StopAllCoroutines();
        StartCoroutine(FadeInBGMVolume(soundEffectDictionary[name]));
    }
    public void StopBGM()
    {
        StopAllCoroutines();
        isStopped = true;
        StartCoroutine(FadeOutBGMVolume());
    }
    private IEnumerator FadeInBGMVolume(AudioClip audioClip)
    {
        if(bgmAudioSource.clip != audioClip)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = audioClip;
            bgmAudioSource.Play();
        }
        bgmAudioSource.volume = 0;
        float start = 0;
        float end = bgmVolume;
        float time = 0;
        float t = 1.0f;
        while(time < 1)
        {
            time += Time.deltaTime / t;
            bgmAudioSource.volume = Mathf.Lerp(start, end, time);
            yield return null;
        }
        isStopped = false;
    }
    private IEnumerator FadeOutBGMVolume()
    {
        bgmAudioSource.volume = bgmVolume;
        float start = bgmVolume;
        float end = 0;
        float time = 0;
        float t = 0.8f;
        while (time < 1)
        {
            time += Time.deltaTime / t;
            bgmAudioSource.volume = Mathf.Lerp(start, end, time);
            yield return null;
        }
        //bgmAudioSource.Stop();
    }
    public void RandomPitch()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
    }
    public void ChangeSFXVolume(float value)
    {
        audioSource.volume = value;
        sfxVolume = value;
    }
    public void ChangeBGMVolue(float value)
    {
        bgmAudioSource.volume = value;
        bgmVolume = value;
    }

}
