using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManage : MonoBehaviour
{
    public Slider VolumeSlider;

    void Start()
    {
        VolumeSlider.value = AreaBGMRandomPlayer.getMaxVolume();
    }


    void Update()
    {
        AreaBGMRandomPlayer.setMaxVolume(VolumeSlider.value);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
