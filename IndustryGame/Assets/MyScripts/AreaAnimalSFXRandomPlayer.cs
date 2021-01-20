using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class AreaAnimalSFXRandomPlayer : MonoBehaviour
{
    private static AreaAnimalSFXRandomPlayer instance;
    private AudioSource audioSource;
    private List<List<AudioClip>> clips;

    public float loopLimit;
    public float loopTime;
    public float loopParam;

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
        loopTime -= Time.deltaTime * loopParam;

        if(instance.audioSource != null && !instance.audioSource.isPlaying && loopTime <= 0f)
        {
            Debug.Log("In update of sfx player");
            SFXChange();
        }
    }

    public static void setAnimalList(List<Animal> animals)
    {
        foreach (var animal in animals)
        {
            instance.clips.Add(animal.sfxAudio);
        }
        Debug.Log("Adding animals, count: " + animals.Count);
    }
    public static void SFXChange()
    {
        List<AudioClip> audioClips = instance.clips[Random.Range(0, instance.clips.Count)]; 
        if(audioClips.Count > 0)
        {
            instance.audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
            instance.audioSource.Play();
            Debug.Log("Animal making sound");
            instance.loopTime = Random.Range(0, instance.loopLimit);
        }
    }

}
