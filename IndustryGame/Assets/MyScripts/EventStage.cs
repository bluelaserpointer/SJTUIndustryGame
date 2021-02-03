using System.Collections.Generic;

public class EventStage
{
    public readonly EventStageSO so;
    public readonly MainEvent mainEvent;
    public string name { get { return so.infoName; } }
    public string description { get { return so.description; } }
    public string descriptionAfterFinish { get { return so.descriptionAfterFinish; } }
    public int contribution { get { return so.contribution; } }
    public List<Animal> showInAnimalsReport { get { return so.showInAnimalsReport; } }
    public bool showInEnvironmentReport { get { return so.showInEnvironmentReport; } }
    public List<EventStageSO> preFinishInfos { get { return so.preFinishEventStages; } }
    public Buff buffBeforeFinish { get { return so.buffBeforeFinish; } }
    public Buff buffAfterFinish { get { return so.buffAfterFinish; } }

    private bool _isAppeared;
    private bool _isFinished;

    public EventStage(EventStageSO so, MainEvent mainEvent)
    {
        this.so = so;
        this.mainEvent = mainEvent;
    }
    public void finish()
    {
        _isFinished = true;
        buffAfterFinish.removed();
        buffBeforeFinish.applied();
    }
    public bool IsFinished()
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
    public void dayIdle()
    {
        if (_isAppeared)
        {
            if (_isFinished)
            {
                if (buffBeforeFinish != null)
                    buffBeforeFinish.idle();
            }
            else
            {
                if (buffAfterFinish != null)
                    buffAfterFinish.idle();
                if (so.IsFinished(mainEvent.region))
                {
                    _isFinished = true;
                    PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(name, descriptionAfterFinish));
                    Stage.AddResourceValue(ResourceType.contribution, contribution);
                }
            }
        }
        else
        {
            if (so.CanAppear(mainEvent))
            {
                _isAppeared = true;
                PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(mainEvent.name + " - " + name, description));
            }
        }
    }
}
