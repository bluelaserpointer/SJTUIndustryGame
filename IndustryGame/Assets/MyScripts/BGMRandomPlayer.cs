using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/BGMRandomPlayer")]
public class BGMRandomPlayer : ScriptableObject
{
    public AudioSource audioSource;
    public List<AudioClip> clips;

    private int clipIndex;

    // Update is called once per frame
    void Update()
    {
        if (clips.Count > 0)
            return;
        if(!audioSource.isPlaying)
        {
            audioSource.clip = clips[clipIndex = Random.Range(0, clips.Count)];
            audioSource.Play();
        }
    }
}
