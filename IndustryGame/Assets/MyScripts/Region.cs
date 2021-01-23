using System.Collections.Generic;

public class Region
{
    private int regionId;
    private List<Area> areas;
    private List<Event> includedEvents;

    public Region(int regionId, ICollection<Area> areas)
    {
        this.regionId = regionId;
        this.areas = new List<Area>(areas);
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
