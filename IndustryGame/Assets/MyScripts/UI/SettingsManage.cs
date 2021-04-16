using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManage : MonoBehaviour
{
    public static SettingsManage instance;
    public Slider VolumeSlider;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        VolumeSlider.value = 0.3f;
    }


    void Update()
    {
        foreach (AudioSource obj in GameObject.FindObjectsOfType<AudioSource>())
        {
            obj.volume = VolumeSlider.value;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
