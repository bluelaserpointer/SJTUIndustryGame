using System.Collections.Generic;
using UnityEngine;

public class MainEvent
{
    private readonly MainEventSO so;
    public readonly Region region;

    public string name { get { return so.eventName; } }
    public string description { get { return so.description; } }
    public string descriptionAfterFinish { get { return so.descriptionAfterFinish; } }
    public int hideLevel { get { return so.hideLevel; } }
    public readonly List<EventStage> eventStages = new List<EventStage>();
    public List<GlobalAction> includedGlobalActions { get { return so.includedGlobalActions; } }
    public List<AreaAction> includedAreaActions { get { return so.includedAreaActions; } }
    public List<Animal> concernedAnimals { get { return so.concernedAnimals; } }
    public int contribution { get { return so.contribution; } }

    private bool _isAppeared;
    private bool _isFinished;
    public MainEvent(MainEventSO so, Region region)
    {
        this.so = so;
        this.region = region;

        //generate eventStages
        foreach(EventStageSO eventStageSO in so.eventStages)
        {
            eventStages.Add(new EventStage(eventStageSO, this));
        }
        //auto reveal
        if (hideLevel == 0)
            reveal();
    }
    public void dayIdle()
    {
        if (!isFinished())
        {
            foreach (EventStage eventStage in eventStages)
            {
                eventStage.dayIdle();
            }
        }
    }
    public void reveal()
    {
        _isAppeared = true;
        PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(name + " @ " + region.name, description));
    }
    public bool isFinished()
    {
        if (_isFinished)
            return true;
        if (!_isAppeared)
            return false;
        bool judge = true;
        foreach (EventStage eventStage in eventStages)
        {
            if (!eventStage.IsFinished())
            {
                judge = false;
                break;
            }
        }
        if (judge)
        {
            _isFinished = true;
            PopUpCanvas.GenerateNewPopUpWindow(new SimplePopUpWindow(name + " @ " + region.name, descriptionAfterFinish));
            Stage.AddResourceValue(ResourceType.contribution, contribution);
            region.UpdateConcernedSpecies();
        }
        return judge;
    }
    public bool isAppeared()
    {
        return _isAppeared;
    }
    public List<EventStage> GetRevealedEventStages()
    {
        return eventStages.FindAll(eventStage => eventStage.isAppeared());
    }
    public List<EventStage> GetRevealedStagesRelatedToAnimal(Animal animal)
    {
        return eventStages.FindAll(eventStage => eventStage.isAppeared() && eventStage.showInAnimalsReport.Contains(animal));
    }
    public List<EventStage> GetRevealedStagesRelatedToEnvironment()
    {
        return eventStages.FindAll(eventStage => eventStage.isAppeared() && eventStage.showInEnvironmentReport);
    }
}
