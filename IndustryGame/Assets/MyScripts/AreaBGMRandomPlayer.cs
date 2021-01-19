using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class AreaBGMRandomPlayer : MonoBehaviour
{
    private static AreaBGMRandomPlayer instance;
    private AudioSource audioSource;
    private List<AudioClip> clips;
    private int clipIndex;
    public Animator bgmAnimator;
    public float waitTime;

    public List<BGMList> dangerBgmLists;
    public List<BGMList> areaBgmLists;
    public BGMList globalBgmList;

    private int dangerType = -1;
    private int environmentType = -1;
    private int currentType = -1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if(audioSource == null)
                audioSource = GetComponent<AudioSource>();
            if(bgmAnimator == null)
                bgmAnimator = GetComponent<Animator>();
            DontDestroyOnLoad(instance);

            SetGlobalBgmList();
        }else{
            Destroy(gameObject);
        }
        //set stage money objective
    }

    public static void SetDangerBgmList(int areaDangerType)
    {
        SetDangerAudioClipsIdx(areaDangerType);
        BgmChange();
    }

    public static void SetAreaBgmList(Area area)
    {
        SetAreaAudioClipsIdx((int)area.environmentType);
        BgmChange();
    }

    // Global: 0, Area: 1, Danger: 2
    public static void SetGlobalBgmList()
    {
        instance.currentType = 0;
        instance.clips = instance.globalBgmList.clips;
        BgmChange();
    }

    public static void SetAreaAudioClipsIdx(int environmentType)
    {
        Debug.Log("Setting area audio, self: " + instance.environmentType + " -> " + environmentType);
        if(instance.currentType != 1 || instance.environmentType != environmentType || instance.environmentType == -1)
        {
            Debug.Log("Setting area audio, self: " + instance.environmentType + " -> " + environmentType);
            instance.currentType = 1;
            instance.clips = instance.areaBgmLists[environmentType].clips;
            instance.environmentType = environmentType;
        }
    }

    public static void SetDangerAudioClipsIdx(int dangerType)
    {
        if(instance.currentType != 2 || instance.dangerType != dangerType || instance.dangerType == -1)
        {
            instance.currentType = 2;
            Debug.Log("Setting danger audio");
            instance.clips = instance.dangerBgmLists[dangerType].clips;
            instance.dangerType = dangerType;
        }
    }

    public static void BgmChange()
    {
        AudioClip clip = instance.clips[instance.clipIndex = Random.Range(0, instance.clips.Count)];

        if(instance.audioSource.clip == null)
        {
            instance.audioSource.clip = clip;
            instance.audioSource.Play();
        }
        Debug.Log("In bgmChange");
        if(instance.audioSource.clip != null && instance.audioSource.clip != clip)
        {
            Debug.Log("Before fade out");
            // instance.StartCorountine.instance.BgmFadeOut(clip);
            instance.StartCoroutine(BgmFadeOut(clip));
        }
    }

    public static IEnumerator BgmFadeOut(AudioClip clip)
    {
        instance.bgmAnimator.SetTrigger("fadeOut");
        Debug.Log("Wait for " + instance.waitTime + " seconds");
        yield return new WaitForSeconds(instance.waitTime);
        instance.audioSource.clip = clip;
        instance.audioSource.Play();
    }
}
