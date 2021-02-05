using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Region
{
    public readonly string name;
    private readonly int regionId;
    private List<Area> areas = new List<Area>();
    private List<MainEvent> includedEvents = new List<MainEvent>();
    private List<Animal> concernedAnimals = new List<Animal>();
    private HexSpiral hexSpiral = new HexSpiral();
    private int reservatedAreaCount;
    private float reservationTime = 1;
    private float baseReservationPower = 1f, reservationProgress;
    private Area baseArea;
    private Area left, right, bottom, top;
    private Vector3 center;
    private Dictionary<Stack<HexCell>, float> lastHighLightedCellAndTime = new Dictionary<Stack<HexCell>, float>();
    private Dictionary<Animal, AmountChangeRecords> animalAmountsRecords = new Dictionary<Animal, AmountChangeRecords>();

    private static readonly NameTemplates regionNameTemplates = Resources.Load<NameTemplates>("NameTemplates/RegionName");

    public Region(int regionId)
    {
        this.regionId = regionId;
        name = regionId == -1 ? "海洋" : regionNameTemplates.pickRandomOne();
    }
    public void FrameIdle()
    {
        foreach(var pair in lastHighLightedCellAndTime.ToList())
        {
            Stack<HexCell> cells = pair.Key;
            if ((lastHighLightedCellAndTime[cells] -= Timer.getTimeSpeed() * Time.deltaTime) < 0) //remove outdated highlight effects
            {
                while (cells.Count > 0)
                {
                    cells.Pop().HighLighted = false;
                }
                lastHighLightedCellAndTime.Remove(cells);
            }
        }
        if (baseArea != null)
        {
            reservationProgress += GetReservationPower() * Timer.getTimeSpeed() * Time.deltaTime;
            //InGameLog.AddLog("reservate: " + reservationProgress);
            Stack<HexCell> lastHighLightedCells = new Stack<HexCell>();
            while (reservationProgress >= reservationTime + concernedAnimals.Count * 0.2f)
            {
                reservationProgress -= reservationTime;
                if (++reservatedAreaCount >= areas.Count)
                {
                    reservationCompleted();
                }
                else
                {
                    HexCell cell = null;
                    int loops = 0;
                    while (++loops < 512)
                    {
                        cell = Stage.GetHexGrid().GetCell(hexSpiral.next());
                        if (cell != null && cell.RegionId == regionId)
                            break;
                    }
                    if (cell == null)
                    {
                        //InGameLog.AddLog("an area is missing", Color.red);
                        reservationCompleted();
                    } else
                    {
                        cell.HighLighted = true;
                        lastHighLightedCells.Push(cell);
                    }
                    //debug
                    //InGameLog.AddLog("base x z: " + baseArea.GetHexCell().coordinates.X + ", " + baseArea.GetHexCell().coordinates.Z + " step: x " + cell.coordinates.X + " z " + cell.coordinates.Z);
                }
            }
            if(lastHighLightedCells.Count > 0)
            {
                lastHighLightedCellAndTime.Add(lastHighLightedCells, 3.0f);
            }
        }
    }
    public void dayIdle()
    {
        if (regionId == -1)
            return;
        foreach (Area area in areas)
        {
            area.dayIdle();
        }
        foreach (MainEvent anEvent in includedEvents)
        {
            anEvent.DayIdle();
        }
    }
    private void reservationCompleted()
    {
        reservatedAreaCount = 0;
        hexSpiral.setCoordinates(baseArea.GetHexCell().coordinates);
        areas.ForEach(area => area.addReservation());
    }
    public void UpdateConcernedSpecies()
    {
        concernedAnimals.Clear();
        foreach (MainEvent mainEvent in includedEvents)
        {
            if (mainEvent.IsAppeared() && !mainEvent.IsFinished())
            {
                foreach (Animal animal in mainEvent.concernedAnimals)
                {
                    if (!concernedAnimals.Contains(animal))
                        concernedAnimals.Add(animal);
                }
            }
        }
        Stage.UpdateConcernedSpecies();
    }
    public List<Animal> GetConcernedSpecies()
    {
        return concernedAnimals;
    }
    public int? GetSpeciesAmountInLatestRecord(Animal animal)
    {
        return animalAmountsRecords.ContainsKey(animal) ? animalAmountsRecords[animal].GetAmountInLatestRecord() : null;
    }
    public int? GetSpeciesChangeInLatestRecord(Animal animal)
    {
        return animalAmountsRecords.ContainsKey(animal) ? animalAmountsRecords[animal].GetChangeInLatestRecord() : null;
    }
    public void AddArea(Area area)
    {
        if (area == null)
            return;
        if (top == null)
        {
            top = bottom = left = right = area;
        } else
        {
            Vector3 pos = area.gameObject.transform.parent.position;
            if (left.transform.position.x > pos.x)
            {
                left = area;
            } else if (right.transform.position.x < pos.x)
            {
                right = area;
            }
            if (bottom.transform.position.z > pos.z)
            {
                bottom = area;
            } else if (top.transform.position.z < pos.z)
            {
                top = area;
            }
        }
        areas.Add(area);
    }
    public void SetBaseArea(Area area)
    {
        baseArea = area;
        area.markBasement.SetActive(true);
        hexSpiral.setCoordinates(baseArea.GetHexCell().coordinates);
    }
    public Area GetBaseArea()
    {
        return baseArea;
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
        foreach (Area area in areas)
        {
            if (area.environmentType.Equals(type))
                ++count;
        }
        return count;
    }
    public void AddEvent(MainEvent anEvent) {
        includedEvents.Add(anEvent);
    }
    public List<MainEvent> GetEvents()
    {
        return includedEvents;
    }
    public List<MainEvent> GetRevealedEvents()
    {
        return includedEvents.FindAll(anEvent => anEvent.IsAppeared());
    }
    public int CountConstructedBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        areas.ForEach(area => count += area.CountConstructedBuilding(buildingInfo));
        return count;
    }
    public int CountBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        areas.ForEach(area => count += area.CountBuilding(buildingInfo));
        return count;
    }
    public void CalculateCenter()
    {
        this.center = new Vector3((left.transform.position.x + right.transform.position.x) / 2, 150f, (top.transform.position.z + bottom.transform.position.z) / 2);
    }

    public Vector3 GetCenter()
    {
        return center;
    }
    public float GetSizeInCamera(Camera camera)
    {
        float xSize = Mathf.Abs(camera.WorldToViewportPoint(left.transform.position).x - camera.WorldToViewportPoint(right.transform.position).x);
        float ySize = Mathf.Abs(camera.WorldToViewportPoint(top.transform.position).y - camera.WorldToViewportPoint(bottom.transform.position).y);
        return Mathf.Max(xSize, ySize);
    }
    public float GetReservationPower()
    {
        float power = baseReservationPower; ;
        foreach(Specialist specialist in GetSpecialistsInRegion())
        {
            power += specialist.GetLevel();
        }
        return power;
    }
    public List<Specialist> GetSpecialistsInRegion()
    {
        return Stage.GetSpecialists().FindAll(specialist =>
        {
            Area area = specialist.getCurrentArea();
            return area != null && area.region.Equals(this);
        });
    }
    public List<EventStage> GetEventInfosRelatedToEnvironment() //TODO: optimize this code
    {
        List<EventStage> eventStages = new List<EventStage>();
        includedEvents.ForEach(anEvent => eventStages.AddRange(anEvent.GetRevealedUnfinishedStagesRelatedToEnvironment()));
        return eventStages;
    }
}
