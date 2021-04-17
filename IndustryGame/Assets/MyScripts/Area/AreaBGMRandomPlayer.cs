using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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



    // Param for volume fade in/out
    public float fadeParam = 0.2f;
    private bool bgmFadeIn = false;
    private bool bgmFadeOut = false;
    private bool bgmChange = false;
    private AudioClip nextClip = null;

    public float maxVolume = 1f;
    public float minVolume = 0f;
    private void Start()
    {
        // fadeTime = fadeLimit;
    }
    private void Update()
    {
        // Debug.Log("In update of bgmRandomPlayer");
        if (bgmFadeIn)
        {
            //Debug.Log("Fading in");
            if (bgmChange)
            {
                bgmFadeOut = true;
                bgmFadeIn = false;
                return;
            }

            // fadeTime -= fadeParam * Time.deltaTime;
            audioSource.volume = Mathf.Lerp(audioSource.volume, maxVolume, Time.deltaTime * fadeParam);

            if (audioSource.volume > maxVolume - 0.05f)
            {
                bgmFadeIn = false;
                // audioSource.volume = maxVolume;

                // fadeTime = fadeLimit;

            }
        }

        if (bgmFadeOut)
        {
            bgmChange = false;
            //Debug.Log("Fading out");

            // fadeTime -= fadeParam * Time.deltaTime;

            audioSource.volume = Mathf.Lerp(audioSource.volume, minVolume, Time.deltaTime * fadeParam);

            if (audioSource.volume < minVolume + 0.05f)
            {
                bgmFadeOut = false;
                bgmFadeIn = true;

                // fadeTime = fadeLimit;

                audioSource.volume = minVolume;

                audioSource.clip = nextClip;
                audioSource.Play();

            }
        }

        if (!bgmFadeOut && !bgmFadeIn)
        {
            audioSource.volume = maxVolume;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            audioSource.volume = maxVolume;
            // if(bgmAnimator == null)
            //     bgmAnimator = GetComponent<Animator>();
            // DontDestroyOnLoad(instance);

            SetGlobalBgmList();
        }
        else
        {
            Destroy(gameObject);
        }
        //set stage money objective
    }

    public static void setMaxVolume(float volume)
    {
        instance.maxVolume = volume;
    }

    public static float getMaxVolume()
    {
        return instance.maxVolume;
    }

    public static void SetDangerBgmList(int areaDangerType)
    {
        SetDangerAudioClipsIdx(areaDangerType);
    }

    public static void SetAreaBgmList(Area area)
    {
        //TODO: update to newest environmentType class form
        //SetAreaAudioClipsIdx((int)area.environmentType);
    }

    public static void SetGlobalBgmList()
    {
        instance.audioType = 0;
        instance.clips = instance.globalBgmList.clips;
        BgmChange();
    }

    public static void SetAreaAudioClipsIdx(int areaType)
    {
        if (instance.audioType != BgmAudioType.Area || instance.areaType != areaType || instance.areaType == -1)
        {
            instance.audioType = BgmAudioType.Area;
            instance.clips = instance.areaBgmLists[(int)instance.audioType].clips;
            instance.areaType = areaType;
            BgmChange();
        }
    }

    public static void SetDangerAudioClipsIdx(int dangerType)
    {
        if (instance.audioType != BgmAudioType.Danger || instance.dangerType != dangerType || instance.dangerType == -1)
        {
            instance.audioType = BgmAudioType.Danger;
            instance.clips = instance.dangerBgmLists[(int)instance.audioType].clips;
            instance.dangerType = dangerType;
            BgmChange();
        }
    }

    public static BgmAudioType GetAudioType()
    {
        return instance.audioType;
    }

    public static void BgmChange()
    {
        if (instance.clips == null || instance.clips.Count <= 0)
            return;

        AudioClip clip = instance.clips[instance.clipIndex = Random.Range(0, instance.clips.Count)];

        if (instance.audioSource.clip == null)
        {
            instance.audioSource.clip = clip;
            instance.audioSource.Play();
        }
        if (instance.audioSource.clip != null && instance.audioSource.clip != clip)
        {
            instance.bgmChange = true;
            instance.bgmFadeOut = true;
            instance.nextClip = clip;
        }
        // instance.StartCoroutine(BgmFadeOut(clip));
    }

    public static IEnumerator BgmFadeOut(AudioClip clip)
    {
        instance.bgmAnimator.SetTrigger("fadeOut");
        Debug.Log(instance.bgmAnimator.GetCurrentAnimatorClipInfo(1).Length);
        yield return new WaitForSeconds(instance.waitTime);
        instance.audioSource.clip = clip;
        instance.audioSource.Play();
    }
}
