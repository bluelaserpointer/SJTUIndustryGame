using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class AreaSFXRandomPlayer : MonoBehaviour
{
    private static AreaSFXRandomPlayer instance;
    private AudioSource audioSource;
    private List<AudioClip> clips;
    private int clipIndex;
    private bool focusChanged = false;

    public Animator sfxAnimator;
    public float waitTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }else{
            Destroy(gameObject);
        }
        //set stage money objective
    }

    public static void setAnimalList(List<Animal> animals)
    {
        List<AudioClip> audioClips = new List<AudioClip>();
        foreach (var animal in animals)
        {
            audioClips.Add(animal.shoutAudio);
        }

        SetAudioClips(audioClips);
        SFXChange();
    }
    public static void SetAudioClips(List<AudioClip> audioClips)
    {
        instance.clips = audioClips;
    }
    public static void SFXChange()
    {
        instance.audioSource.clip = instance.clips[instance.clipIndex = Random.Range(0, instance.clips.Count)];
        instance.audioSource.Play();
        instance.SFXFadeOut();
    }
    IEnumerator SFXFadeOut()
    {
        sfxAnimator.SetTrigger("fadeOut");
        yield return new WaitForSeconds(waitTime);
    }
}
