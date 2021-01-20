using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AreaDetailsHUD : MonoBehaviour
{
    private Area CurrentArea;
    

    void Start()
    {
        
    }

    void Update()
    {
        CurrentArea = OrthographicCamera.GetMousePointingArea();    
    }

}
