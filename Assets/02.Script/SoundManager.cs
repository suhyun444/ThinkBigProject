using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{

    public static SoundManager instance = null;
    [SerializeField] SoundData soundSetting;

    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private AudioSource bgmAudioSource = null;
    private Dictionary<string, AudioClip> soundEffectDictionary;
    private float sfxVolume = 1.0f;
    private float bgmVolume = 1.0f;
    private bool isStopped = false;
    public bool isOnTeleport = false;
    public bool isDead = false;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            soundEffectDictionary = new Dictionary<string, AudioClip>();
            InitSoundDictionary();
            //audioSource.volume = ES3.Load("SFXVolume", 1.0f);
            sfxVolume = audioSource.volume;
            //bgmAudioSource.volume = ES3.Load("BGMVolume", 1.0f);
            bgmVolume = bgmAudioSource.volume;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void InitSoundDictionary()
    {
        int count = soundSetting.count;
        for(int i = 0; i < count; i++)
        {
            soundEffectDictionary.Add(soundSetting.nameList[i], soundSetting.clipList[i]);
        }
    }

    public void PlaySoundEffect(string name)
    {
        if (isDead) return;
        if (!soundEffectDictionary.ContainsKey(name))
        {
            Debug.LogError($"{name} is not available");
            return;
        }
        RandomPitch();
        audioSource.PlayOneShot(soundEffectDictionary[name]);
    }
    public void PlayBGM(string name)
    {
        if (isOnTeleport) return;
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
        if (isOnTeleport) return;
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
        float t = 2;
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
        float t = 1.5f;
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
        audioSource.pitch = Random.Range(0.9f, 1.1f);
    }
    public void ChangeSFXVolume(float value)
    {
        Debug.Log(value);
        audioSource.volume = value;
        sfxVolume = value;
        //ES3.Save("SFXVolume", value);
    }
    public void ChangeBGMVolue(float value)
    {
        if (!isStopped) bgmAudioSource.volume = value   ;
        bgmVolume = value;
        //ES3.Save("BGMVolume", value);
    }

}
