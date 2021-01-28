using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ActionsPanelUI : MonoBehaviour
{
    private Area CurrentArea;
    public Dropdown Actions;
    public Dropdown Specialists;

    void Update()
    {
        CurrentArea = OrthographicCamera.GetMousePointingArea();
        if (CurrentArea != null)
        {
            UpdateActions();
            UpdateSpecialists();
        }

        //InGameLog.AddLog("Action progress rate: " + Stage.GetSpecialists()[Specialists.value].getActionProgressRate().ToString());
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
            foreach (Specialist specialist in Stage.GetSpecialists())
            {
                Dropdown.OptionData tempData = new Dropdown.OptionData();
                tempData.text = specialist.name + "   " + specialist.GetCurrentAreaName();
                //tempData.image = CurrentArea.GetEnabledActions()[i].actionName;
                Specialists.options.Add(tempData);
                //InGameLog.AddLog(Stage.GetSpecialists()[i].name);
            }
            Specialists.captionText.text = Stage.GetSpecialists()[Specialists.value].name + "   " + Stage.GetSpecialists()[Specialists.value].GetCurrentAreaName();
        }
        
    }

    public void StartAction ()
    {
        Stage.GetSpecialists()[Specialists.value].moveToArea(CurrentArea);
        Stage.GetSpecialists()[Specialists.value].startAction(CurrentArea.GetEnabledActions()[Actions.value]);
    }




}
