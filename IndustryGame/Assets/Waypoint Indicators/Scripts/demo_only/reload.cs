using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reload : MonoBehaviour
{
    //float time;

    void OnEnable()
    {
        event_manager.onCloseOptionScreen += reloadObject;
        event_manager.onStartLevel += reloadObject;
    }

    void OnDisable()
    {
        event_manager.onCloseOptionScreen -= reloadObject;
        event_manager.onStartLevel -= reloadObject;
    }

    void reloadObject()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
