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

    public void UpdateActions()
    {
        Actions.options.Clear();
        if (CurrentArea.GetEnabledActions().Count > 0)
        {
            for (int i = 0; i < CurrentArea.GetEnabledActions().Count; i++)
            {
                Dropdown.OptionData tempData = new Dropdown.OptionData();
                tempData.text = CurrentArea.GetEnabledActions()[i].name;
                //tempData.image = CurrentArea.GetEnabledActions()[i].name;
                Actions.options.Add(tempData);
                //InGameLog.AddLog(CurrentArea.GetEnabledActions()[i].name + " test ");
            }

            Actions.captionText.text = CurrentArea.GetEnabledActions()[Actions.value].name;
        }
    }

    public void UpdateSpecialists()
    {
        Specialists.options.Clear();

        if (Stage.GetSpecialists().Count > 0)
        {
            foreach (Specialist specialist in Stage.GetSpecialists())
            {
                Dropdown.OptionData tempData = new Dropdown.OptionData();
                tempData.text = specialist.name + "   " + specialist.GetCurrentAreaName();
                //tempData.image = CurrentArea.GetEnabledActions()[i].name;
                Specialists.options.Add(tempData);
                //InGameLog.AddLog(Stage.GetSpecialists()[i].name);
            }
            Specialists.captionText.text = Stage.GetSpecialists()[Specialists.value].name + "   " + Stage.GetSpecialists()[Specialists.value].GetCurrentAreaName();
        }

    }

    public void StartAction()
    {
        Stage.GetSpecialists()[Specialists.value].MoveToArea(CurrentArea);
        //TODO: edit
        //Stage.GetSpecialists()[Specialists.value].SetAction(CurrentArea.GetEnabledActions()[Actions.value]);
    }




}
