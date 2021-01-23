using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/EventInfo")]
public class EventInfo : ScriptableObject
{
    public string infoName;
    [TextArea]
    public string description;
    [TextArea]
    public string descriptionAfterFinish;

    [Header("完成功劳奖励")]
    [Min(0)]
    public int contribution;

    [Header("出现在以下动物报告")]
    public List<Animal> showInAnimalsReport;
    [Header("出现在环境报告")]
    public bool showInEnvironmentReport;
    [Header("出现前需完成的EventInfo")]
    public List<EventInfo> preFinishInfos;
    [Header("完成条件")]
    public Condition successCondition;
    [Header("完成前的环境效果")]
    public Buff buffBeforeFinish;
    [Header("完成后的环境效果")]
    public Buff buffAfterFinish;

    private bool _isAppeared;
    private bool _isFinished;
    public void finish()
    {
        _isFinished = true;
        buffAfterFinish.removed();
        buffBeforeFinish.applied();
    }
    public bool isFinished()
    {
        return _isFinished;
    }
    public bool isAppeared()
    {
        return _isAppeared;
    }
    public string getDescription()
    {
        return _isFinished ? descriptionAfterFinish : description;
    }
    public void init()
    {
        _isAppeared = _isFinished = false;
    }
    public void dayIdle()
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
            {
                _isAppeared = true;
                PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(infoName, description));
            }
        }
        if (_isFinished) {
            if (buffBeforeFinish != null)
                buffBeforeFinish.idle();
        } else {
            if (buffAfterFinish != null)
                buffAfterFinish.idle();
            if(successCondition == null || successCondition.judge())
            {
                _isFinished = true;
                PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(infoName, descriptionAfterFinish));
                Stage.contribution += contribution;
            }
        }
    }
}