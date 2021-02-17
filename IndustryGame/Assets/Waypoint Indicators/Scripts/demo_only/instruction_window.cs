using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class instruction_window : MonoBehaviour
{
    public GameObject btn_Day;
    public GameObject btn_Day_Selected;
    public GameObject indicator_Day;
    public GameObject btn_Night;
    public GameObject btn_Night_Selected;
    public GameObject indicator_Night;



    void OnEnable()
    {
        event_manager.onLoadScene += SwapSceneButtonStates;
    }

    void OnDisable()
    {
        event_manager.onLoadScene -= SwapSceneButtonStates;
    }


    void SwapSceneButtonStates(int levelToLoad)
    {
        if (levelToLoad == 1)
        {
            //Highlight Day Scene
            btn_Day.SetActive(false);
            btn_Day_Selected.SetActive(true);
            indicator_Day.SetActive(true);
            btn_Night.SetActive(true);
            btn_Night_Selected.SetActive(false);
            indicator_Night.SetActive(false);
        }
        if (levelToLoad == 2)
        {
            //Highlight Night Scene
            btn_Day.SetActive(true);
            btn_Day_Selected.SetActive(false);
            indicator_Day.SetActive(false);
            btn_Night.SetActive(false);
            btn_Night_Selected.SetActive(true);
            indicator_Night.SetActive(true);
        }
    }


    void Start()
    {
        //When this loads, check to see what the levelNum is then show buttons accordingly
        if (scene_manager.curLevel == 1)
        {
            //Highlight Day Scene
            btn_Day.SetActive(false);
            btn_Day_Selected.SetActive(true);
            indicator_Day.SetActive(true);
            btn_Night.SetActive(true);
            btn_Night_Selected.SetActive(false);
            indicator_Night.SetActive(false);
        }
        if (scene_manager.curLevel == 2)
        {
            //Highlight Night Scene
            btn_Day.SetActive(true);
            btn_Day_Selected.SetActive(false);
            indicator_Day.SetActive(false);
            btn_Night.SetActive(false);
            btn_Night_Selected.SetActive(true);
            indicator_Night.SetActive(true);
        }
    }

}
