using System.Collections.Generic;

public class Region
{
    private int regionId;
    private List<Area> areas = new List<Area>();
    private List<Event> includedEvents;

    public Region(int regionId)
    {
        this.regionId = regionId;
    }
    public void AddArea(Area area)
    {
        areas.Add(area);
    }
    public int GetRegionId()
    {
        return regionId;
    }
    public List<Area> GetAreas()
    {
        return areas;
    }
    public int CountEnvironmentType(EnvironmentType type)
    {
        int count = 0;
        foreach(Area area in areas)
        {
            if (area.environmentType.Equals(type))
                ++count;
        }
        return count;
    }
    public void AddEvent(Event anEvent) {
        includedEvents.Add(anEvent);
    }
    public List<Event> GetEvents()
    {
        return includedEvents;
    }
    public List<Event> GetRevealedEvents()
    {
        return includedEvents.FindAll(anEvent => anEvent.isAppeared());
    }
}
