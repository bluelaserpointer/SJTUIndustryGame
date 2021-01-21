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

    public List<AudioClipList> dangerBgmLists;
    public List<AudioClipList> areaBgmLists;
    public AudioClipList globalBgmList;

    private BgmAudioType audioType = BgmAudioType.None;
    private int areaType = -1;
    private int dangerType = -1;
    public enum BgmAudioType
    {
        None = -1,
        Global,
        Area,
        Danger,

    }

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
    }

    public static void SetAreaBgmList(Area area)
    {
        SetAreaAudioClipsIdx((int)area.environmentType);
    }

    public static void SetGlobalBgmList()
    {
        instance.audioType = 0;
        instance.clips = instance.globalBgmList.clips;
        BgmChange();
    }

    public static void SetAreaAudioClipsIdx(int areaType)
    {
        if(instance.audioType != BgmAudioType.Area || instance.areaType != areaType || instance.areaType == -1)
        {
            instance.audioType = BgmAudioType.Area;
            instance.clips = instance.areaBgmLists[(int)instance.audioType].clips;
            instance.areaType = areaType;
            BgmChange();
        }
    }

    public static void SetDangerAudioClipsIdx(int dangerType)
    {
        if(instance.audioType != BgmAudioType.Danger || instance.dangerType != dangerType || instance.dangerType == -1)
        {
            instance.audioType = BgmAudioType.Danger;
            instance.clips = instance.dangerBgmLists[(int)instance.audioType].clips;
            instance.dangerType = dangerType;
            BgmChange();
        }
    }

    public static void BgmChange()
    {
        if(instance.clips == null || instance.clips.Count <= 0)
            return;
            
        AudioClip clip = instance.clips[instance.clipIndex = Random.Range(0, instance.clips.Count)];

        if(instance.audioSource.clip == null)
        {
            instance.audioSource.clip = clip;
            instance.audioSource.Play();
        }
        if(instance.audioSource.clip != null && instance.audioSource.clip != clip)
            instance.StartCoroutine(BgmFadeOut(clip));
    }

    public static IEnumerator BgmFadeOut(AudioClip clip)
    {
        instance.bgmAnimator.SetTrigger("fadeOut");
        yield return new WaitForSeconds(instance.waitTime);
        instance.audioSource.clip = clip;
        instance.audioSource.Play();
    }
}
