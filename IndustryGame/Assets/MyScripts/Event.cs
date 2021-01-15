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
    public void idle()
    {
        foreach(EventInfo info in includedInfos) {
            info.idle();
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
}
