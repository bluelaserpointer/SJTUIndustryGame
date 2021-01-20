using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AreaDetailsHUD : MonoBehaviour
{
    private Area CurrentArea;
    public Dropdown Actions;
    public Dropdown Specialists;

    

    void Start()
    {
        
    }

    void Update()
    {
        CurrentArea = OrthographicCamera.GetMousePointingArea();
        UpdateActions();
        UpdateSpecialists();
    }

    public void UpdateActions ()
    {
        Actions.options.Clear();
        if (CurrentArea.GetEnabledActions().Count > 0)
        {
            for (int i = 0 ; i < CurrentArea.GetEnabledActions().Count ; i++)
            {
                Dropdown.OptionData tempData = new Dropdown.OptionData();
                tempData.text = CurrentArea.GetEnabledActions()[i].actionName;
                //tempData.image = CurrentArea.GetEnabledActions()[i].actionName;
                Actions.options.Add(tempData);
                //InGameLog.AddLog(CurrentArea.GetEnabledActions()[i].actionName + " test ");
            }

            Actions.captionText.text = CurrentArea.GetEnabledActions()[Actions.value].actionName;
        }
    }

    public void UpdateSpecialists ()
    {
        Specialists.options.Clear();

        if (Stage.GetSpecialists().Count > 0)
        {
            for (int i = 0 ; i < Stage.GetSpecialists().Count ; i++)
            {
                Dropdown.OptionData tempData = new Dropdown.OptionData();
                tempData.text = Stage.GetSpecialists()[i].name + "   " + Stage.GetSpecialists()[i].getCurrentArea().name;
                //tempData.image = CurrentArea.GetEnabledActions()[i].actionName;
                Specialists.options.Add(tempData);
                //InGameLog.AddLog(Stage.GetSpecialists()[i].name);
            }
            Specialists.captionText.text = Stage.GetSpecialists()[Specialists.value].name + "   " + Stage.GetSpecialists()[Specialists.value].getCurrentArea().name;
        }
        
    }

    public void StartAction ()
    {
        Stage.GetSpecialists()[Specialists.value].startAction(CurrentArea.GetEnabledActions()[Actions.value]);
        Stage.GetSpecialists()[Specialists.value].moveToArea(CurrentArea);
    }
}
