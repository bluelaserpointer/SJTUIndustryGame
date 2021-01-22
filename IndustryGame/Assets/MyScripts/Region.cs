using System.Collections.Generic;

public class Region
{
    private int regionId;
    private List<Area> areas;

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
}
