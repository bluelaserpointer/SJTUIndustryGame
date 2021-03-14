using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;
    public float volume = 0.1f;
    public AudioClip MouseClickClip;
    private AudioSource ClickAudioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ClickAudioSource = gameObject.GetComponent<AudioSource>();
        ClickAudioSource.clip = MouseClickClip;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickAudioSource.Play();
        }
    }

}
