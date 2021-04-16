using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSfxPlayer : MonoBehaviour
{
    public AudioClip startConstructionClip, endConstructionClip;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartConstruction()
    {
        audioSource.clip = startConstructionClip;
        audioSource.Play();
    }

    public void EndConstruction()
    {
        audioSource.clip = endConstructionClip;
        audioSource.Play();
    }
}
