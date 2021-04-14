using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class AreaAnimalSFXRandomPlayer : MonoBehaviour
{
    private static AreaAnimalSFXRandomPlayer instance;
    private AudioSource audioSource;
    private List<List<AudioClipList>> clips;

    public float loopLimit;
    public float loopTime;
    public float loopParam;

    public float currVolume = 1f;

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

        loopTime -= Time.deltaTime * loopParam;

        if(instance.audioSource != null && !instance.audioSource.isPlaying && loopTime <= 0f)
        {
            SFXChange();
            instance.loopTime = 0f;
        }
    }

    public static void setCurrVolume(float volume)
    {
        instance.currVolume = volume;
    }

    public static void setAnimalList(List<List<Animal>> speciesDangerTypes)
    {
        if(speciesDangerTypes.Count <= 0)
            return;

        // InGameLog.AddLog("In setAnimalList");
        List<List<AudioClipList>> dangerTypesList = new List<List<AudioClipList>>();
        SpeciesDangerType mostDangerType = (SpeciesDangerType)EnumHelper.GetMaxEnum<SpeciesDangerType>();
        for(int i = 0; i <= (int)mostDangerType; i++)
            dangerTypesList.Add(new List<AudioClipList>());

        for (SpeciesDangerType i = 0; i <= mostDangerType; i++)
        {
            List<AudioClipList> currDangerTypeList = new List<AudioClipList>();
            List<Animal> currDangerTypeAnimals = speciesDangerTypes[(int)i];
            foreach (Animal species in currDangerTypeAnimals)
            {
                currDangerTypeList.Add(species.sfxAudio);
            }

            // InGameLog.AddLog("CurrDangerTypeList Count: " + currDangerTypeList.Count);
            dangerTypesList[(int)i] = currDangerTypeList;
        }
        instance.clips = dangerTypesList;
    }
    public static void SFXChange()
    {
        if(instance.clips == null || instance.clips.Count <= 0)
            return;

        SpeciesDangerType currDangerType = (SpeciesDangerType)Random.Range(0, instance.clips.Count);
        SpeciesDangerType mostDangerType = (SpeciesDangerType)EnumHelper.GetMaxEnum<SpeciesDangerType>();
        List<AudioClipList> currDangerTypeList = instance.clips[(int)currDangerType]; 
        if(currDangerTypeList.Count > 0)
        {
            AudioClipList currDangerTypeAnimal = currDangerTypeList[Random.Range(0, currDangerTypeList.Count)];
            if(currDangerTypeAnimal.clips.Count > 0)
            {
                // InGameLog.AddLog("SFXChange: currDangerType " + (int)currDangerType + " mostDangerType " + (int)mostDangerType);

                // Set audioSource & audioVolumn
                // InGameLog.AddLog(currDangerTypeAnimal.clips.Count.ToString());
                instance.audioSource.clip = currDangerTypeAnimal.clips[Random.Range(0, currDangerTypeAnimal.clips.Count)];
                instance.audioSource.volume = 1 - (float)((float)currDangerType / (float)mostDangerType) + 0.2f;
                instance.loopTime = Random.Range(0, instance.loopLimit);
                if(AreaBGMRandomPlayer.GetAudioType() == AreaBGMRandomPlayer.BgmAudioType.Area)
                    instance.audioSource.Play();
            }else{
                instance.loopTime = 0f;
            }
        }else{
            instance.loopTime = 0f;
        }
    }

}
