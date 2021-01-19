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
    public void dayIdle()
    {
        foreach(EventInfo info in includedInfos) {
            info.dayIdle();
        }
    }
    public bool isFinished()
    {
        bool judge = true;
        foreach (EventInfo info in includedInfos)
        {
            if(!info.isFinished())
            {
                judge = false;
                break;
            }
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
