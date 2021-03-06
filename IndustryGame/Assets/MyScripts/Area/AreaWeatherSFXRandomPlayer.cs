﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class AreaWeatherSFXRandomPlayer : MonoBehaviour
{
    private static AreaWeatherSFXRandomPlayer instance;
    private AudioSource audioSource;
    private List<AudioClip> clips;
    public List<AudioClipList> weatherSfxList;
    private int clipIndex;
    public float loopLimit;
    public float loopTime;
    public float loopParam;
    public Weather.WeatherType currentWeatherType = Weather.WeatherType.Sunny;
    private bool focus = false;
    private float currVolume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            if(audioSource == null)
                audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(instance);

        }else{
            Destroy(gameObject);
        }
        //set stage money objective
    }

    private void Update() {
        if(audioSource.volume != currVolume)
            audioSource.volume = currVolume;

    }
    public static void setCurrVolume(float volume)
    {
        instance.currVolume = volume;
    }

    public static void setArea(Area currentArea)
    {
        Weather.WeatherType weatherType = currentArea.GetWeather().GetWeatherType();
        if(instance.currentWeatherType != weatherType || instance.focus == false)
        {
            instance.currentWeatherType = weatherType;
            AudioClipList audioClips = instance.weatherSfxList[(int)(instance.currentWeatherType)];
            
            SetAudioClips(audioClips);
            SFXChange();
        }
    }
    public static void SetAudioClips(AudioClipList audioClips)
    {
        instance.clips = audioClips.clips;
    }
    public static void SFXChange()
    {
        if(instance.clips.Count > 0)
        {
            instance.audioSource.clip = instance.clips[Random.Range(0, instance.clips.Count)];
            instance.audioSource.Play();
            // Debug.Log("Animal making sound");
        }else{
            Silence();
        }
        // instance.loopTime = Random.Range(0, instance.loopLimit);
    }

    public static void Silence()
    {
        instance.focus = false;
        instance.audioSource.Stop();
    }

}
