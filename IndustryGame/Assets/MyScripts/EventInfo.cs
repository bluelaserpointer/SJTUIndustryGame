using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Event - Info")]
public class EventInfo : ScriptableObject
{
    public string infoName;
    [TextArea]
    public string description;
    [TextArea]
    public string descriptionAfterFinish;
    [Header("出现前需完成的EventInfo")]
    public List<EventInfo> preFinishInfos;
    [Header("完成条件")]
    public Condition successCondition;
    [Header("完成后能去除的环境效果")]
    public Buff buff;
    [Header("完成前提供的可用措施")]
    public List<Action> givenActionsBeforeSolve;
    [Header("完成后提供的可用措施")]
    public List<Action> givenActionsAfterSolve;

    private bool _isAppeared;
    private bool _isFinished;
    public void finish()
    {
        _isFinished = true;
        buff.removed();
    }
    public bool isFinished()
    {
        return _isFinished;
    }
    public string getDescription()
    {
        return _isFinished ? descriptionAfterFinish : description;
    }
    public void idle()
    {
        if (!_isAppeared)
        {
            bool shouldAppear = true;
            foreach (EventInfo info in preFinishInfos)
            {
                if (!info.isFinished())
                {
                    shouldAppear = false;
                    break;
                }
            }
            if (shouldAppear)
                _isAppeared = true;
        }
        if (!_isFinished && buff != null)
        {
            buff.idle();
        }
    }
}