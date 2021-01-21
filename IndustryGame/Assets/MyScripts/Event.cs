using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Event")]
public class Event : ScriptableObject
{
    public string eventName;
    [TextArea]
    public string description;
    [TextArea]
    public string descriptionAfterFinish;

    [Header("出现的所有Info")]
    public List<EventInfo> includedInfos = new List<EventInfo>();
    [Header("出现的所有全局措施")]
    public List<GlobalAction> includedGlobalActions = new List<GlobalAction>();
    [Header("出现的所有地区措施")]
    public List<AreaAction> includedAreaActions = new List<AreaAction>();

    private bool _isFinished;
    public void init()
    {
        foreach(EventInfo info in includedInfos)
        {
            info.init();
        }
        PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(eventName, description));
    }
    public void dayIdle()
    {
        foreach(EventInfo info in includedInfos) {
            info.dayIdle();
        }
    }
    public bool isFinished()
    {
        if (_isFinished)
            return true;
        bool judge = true;
        foreach (EventInfo info in includedInfos)
        {
            if(!info.isFinished())
            {
                judge = false;
                break;
            }
        }
        if(judge)
        {
            _isFinished = true;
            PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(eventName, descriptionAfterFinish));
        }
        return judge;
    }
    public List<EventInfo> GetInfos()
    {
        return includedInfos;
    }
    public List<EventInfo> GetRevealedInfos()
    {
        return includedInfos.FindAll(info => info.isAppeared());
    }
    public List<EventInfo> GetRevealedInfosRelatedToAnimal(Animal animal)
    {
        return includedInfos.FindAll(info => info.isAppeared() && info.showInAnimalsReport.Contains(animal));
    }
    public List<EventInfo> GetRevealedInfosRelatedToEnvironment()
    {
        return includedInfos.FindAll(info => info.isAppeared() && info.showInEnvironmentReport);
    }
}
