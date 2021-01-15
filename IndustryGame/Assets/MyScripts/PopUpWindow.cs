using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpWindow : MonoBehaviour
{
    private static PopUpWindow staticInstance;
    public void Awake()
    {
        if(staticInstance == null)
        {
            staticInstance = this;
        }
    }
    public static void show()
    {
        staticInstance.transform.gameObject.SetActive(true);
    }
}
