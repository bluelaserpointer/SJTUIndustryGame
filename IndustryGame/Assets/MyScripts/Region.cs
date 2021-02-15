using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 洲
/// </summary>
public class Region
{
    /// <summary>
    /// 洲名称
    /// </summary>
    public readonly string name;
    /// <summary>
    /// 洲序号
    /// </summary>
    public readonly int regionId;
    /// <summary>
    /// 是否为海洋
    /// </summary>
    public bool IsOcean { get { return regionId == -1; } }
    private readonly List<Area> areas = new List<Area>();
    private readonly List<MainEvent> includedEvents = new List<MainEvent>();
    private readonly List<Animal> concernedAnimals = new List<Animal>();
    private readonly HexSpiral hexSpiral = new HexSpiral();
    private int reservatedAreaCount;
    private float reservationTime = 1;
    private float baseReservationPower = 2f, reservationProgress;
    private Area baseArea;
    private Area left, right, bottom, top;
    private Vector3 center;
    public float observeOrthoSize;
    private Dictionary<Stack<HexCell>, float> lastHighLightedCellAndTime = new Dictionary<Stack<HexCell>, float>();

    private static readonly NameTemplates regionNameTemplates = Resources.Load<NameTemplates>("NameTemplates/RegionName");

    public Region(int regionId)
    {
        this.regionId = regionId;
        name = regionId == -1 ? "海洋" : regionNameTemplates.pickRandomOne();
    }
    /// <summary>
    /// 每帧流程
    /// </summary>
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
                        InGameLog.AddLog("an area is missing", Color.red);
                        reservationCompleted();
                    } else
                    {
                        //reservate progress debug
                        InGameLog.AddLog("reservate: " + reservatedAreaCount + "/" + areas.Count);
                        cell.HighLighted = true;
                        lastHighLightedCells.Push(cell);
                    }
                }
            }
            if(lastHighLightedCells.Count > 0)
            {
                lastHighLightedCellAndTime.Add(lastHighLightedCells, 3.0f);
            }
        }
    }
    /// <summary>
    /// 每日流程
    /// </summary>
    public void DayIdle()
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
    /// <summary>
    /// 结束调查
    /// </summary>
    private void reservationCompleted()
    {
        reservatedAreaCount = 1; // base area is always reservated
        hexSpiral.setCoordinates(baseArea.GetHexCell().coordinates);
        areas.ForEach(area => area.addReservation());
    }
    /// <summary>
    /// 更新洲濒危动物列表(每当一个事件流开始/结束时被调用)
    /// </summary>
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
    /// <summary>
    /// 获取洲濒危动物列表
    /// </summary>
    /// <returns></returns>
    public List<Animal> GetConcernedSpecies()
    {
        return concernedAnimals;
    }
    /// <summary>
    /// 获取最新统计内指定动物数量
    /// </summary>
    /// <param name="animal"></param>
    /// <returns></returns>
    public int? GetSpeciesAmountInLatestRecord(Animal animal)
    {
        int sum = 0;
        bool hasRecord = false;
        foreach(Area area in areas)
        {
            int? value = area.GetSpeciesAmountInLatestRecord(animal);
            if(value.HasValue)
            {
                hasRecord = true;
                sum += value.Value;
            }
        }
        return hasRecord ? (int?)sum : null;
    }
    /// <summary>
    /// 获取最新统计内指定动物增减量
    /// </summary>
    public int? GetSpeciesChangeInLatestRecord(Animal animal)
    {
        int sum = 0;
        bool hasRecord = false;
        foreach (Area area in areas)
        {
            int? value = area.GetSpeciesChangeInLatestRecord(animal);
            if (value.HasValue)
            {
                hasRecord = true;
                sum += value.Value;
            }
        }
        return hasRecord ? (int?)sum : null;
    }
    /// <summary>
    /// 添加地区
    /// </summary>
    /// <param name="area"></param>
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
    /// <summary>
    /// 设置基地地区
    /// </summary>
    /// <param name="area"></param>
    public void SetBaseArea(Area area)
    {
        baseArea = area;
        area.markBasement.SetActive(true);
        hexSpiral.setCoordinates(baseArea.GetHexCell().coordinates);
        reservatedAreaCount = 1; //base area is always reservated
    }
    /// <summary>
    /// 获取基地地区
    /// </summary>
    /// <returns></returns>
    public Area GetBaseArea()
    {
        return baseArea;
    }
    /// <summary>
    /// 获取洲序号
    /// </summary>
    /// <returns></returns>
    public int GetRegionId()
    {
        return regionId;
    }
    /// <summary>
    /// 获取所含地区
    /// </summary>
    /// <returns></returns>
    public List<Area> GetAreas()
    {
        return areas;
    }
    /// <summary>
    /// 统计指定环境种类数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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
    /// <summary>
    /// 添加事件流
    /// </summary>
    /// <param name="anEvent"></param>
    public void AddEvent(MainEvent anEvent) {
        includedEvents.Add(anEvent);
    }
    /// <summary>
    /// 获取事件流
    /// </summary>
    /// <returns></returns>
    public List<MainEvent> GetEvents()
    {
        return includedEvents;
    }
    /// <summary>
    /// 获取已揭开事件流
    /// </summary>
    /// <returns></returns>
    public List<MainEvent> GetRevealedEvents()
    {
        return includedEvents.FindAll(anEvent => anEvent.IsAppeared());
    }
    /// <summary>
    /// 统计指定类型已建设建筑物数量
    /// </summary>
    /// <param name="buildingInfo"></param>
    /// <returns></returns>
    public int CountConstructedBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        areas.ForEach(area => count += area.CountConstructedBuilding(buildingInfo));
        return count;
    }
    /// <summary>
    /// 统计指定类型建筑物数量
    /// </summary>
    /// <param name="buildingInfo"></param>
    /// <returns></returns>
    public int CountBuilding(BuildingInfo buildingInfo)
    {
        int count = 0;
        areas.ForEach(area => count += area.CountBuilding(buildingInfo));
        return count;
    }
    /// <summary>
    /// 重新计算洲中心坐标
    /// </summary>
    public void CalculateCenter()
    {
        center = new Vector3((left.transform.position.x + right.transform.position.x) / 2, 150f, (top.transform.position.z + bottom.transform.position.z) / 2);
    }
    /// <summary>
    /// 获取洲中心坐标
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCenter()
    {
        return center;
    }
    /// <summary>
    /// 获取洲左端坐标
    /// </summary>
    /// <returns></returns>
    public float GetLeft()
    {
        return left.transform.position.x;
    }

    /// <summary>
    /// 获取洲右端坐标
    /// </summary>
    /// <returns></returns>
    public float GetRight()
    {
        return right.transform.position.x;
    }

    /// <summary>
    /// 获取洲上端坐标
    /// </summary>
    /// <returns></returns>
    public float GetTop()
    {
        return top.transform.position.z;
    }

    /// <summary>
    /// 获取洲下端坐标
    /// </summary>
    /// <returns></returns>
    public float GetBottom()
    {
        return bottom.transform.position.z;
    }


    /// <summary>
    /// 获取洲大小
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    public float GetSizeInCamera(Camera camera)
    {
        float xSize = Mathf.Abs(camera.WorldToViewportPoint(left.transform.position).x - camera.WorldToViewportPoint(right.transform.position).x);
        float ySize = Mathf.Abs(camera.WorldToViewportPoint(top.transform.position).y - camera.WorldToViewportPoint(bottom.transform.position).y);
        return Mathf.Max(xSize, ySize);
    }
    /// <summary>
    /// 获取洲调查每日进度
    /// </summary>
    /// <returns></returns>
    public float GetReservationPower()
    {
        float power = baseReservationPower; ;
        foreach(Specialist specialist in GetSpecialistsInRegion())
        {
            power += specialist.GetLevel();
        }
        return power;
    }
    /// <summary>
    /// 获取州内所有专家
    /// </summary>
    /// <returns></returns>
    public List<Specialist> GetSpecialistsInRegion()
    {
        return Stage.GetSpecialists().FindAll(specialist =>
        {
            Area area = specialist.GetCurrentArea();
            return area != null && area.region.Equals(this);
        });
    }
    /// <summary>
    /// 获取洲内环境相关报告
    /// </summary>
    /// <returns></returns>
    public List<EventStage> GetEventInfosRelatedToEnvironment() //TODO: optimize this code
    {
        List<EventStage> eventStages = new List<EventStage>();
        includedEvents.ForEach(anEvent => eventStages.AddRange(anEvent.GetRevealedStagesRelatedToEnvironment()));
        return eventStages;
    }
}
