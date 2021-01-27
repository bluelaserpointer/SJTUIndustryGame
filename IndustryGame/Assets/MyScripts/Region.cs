using System.Collections.Generic;
using UnityEngine;

public class Region
{
    private int regionId;
    private List<Area> areas = new List<Area>();
    private List<MainEvent> includedEvents;
    private HexSpiral hexSpiral = new HexSpiral();
    private int reservatedAreaCount;
    private int reservationTime = 2;
    private int reservationSpeed = 1, reservationProgress;
    private Area baseArea;
    private Area left, right, bottom, top;
    private Vector3 center;
    public Region(int regionId)
    {
        this.regionId = regionId;
    }
    public void dayIdle()
    {
        foreach(Area area in areas)
        {
            area.dayIdle();
        }
        if (baseArea != null)
        {
            reservationProgress += reservationSpeed;
            //InGameLog.AddLog("reservate: " + reservationProgress);
            if (reservationProgress >= reservationTime)
            {
                reservationProgress = 0;
                if (++reservatedAreaCount >= areas.Count)
                {
                    reservationCompleted();
                }
                else
                {
                    HexCell cell = null;
                    int loops = 0;
                    while (++loops < 2048)
                    {
                        cell = Stage.GetHexGrid().GetCell(hexSpiral.next());
                        if (cell != null && cell.RegionId == regionId)
                            break;
                    }
                    //InGameLog.AddLog("step: x " + cell.coordinates.X + " z " + cell.coordinates.Z);
                    if (loops == 2048)
                    {
                        InGameLog.AddLog("an area is missing", Color.red);
                        reservationCompleted();
                    }
                }
            }
            if (reservatedAreaCount >= areas.Count)
            {
                reservatedAreaCount = 0;
            }
        }
    }
    private void reservationCompleted()
    {
        reservatedAreaCount = 0;
        hexSpiral.setCoordinates(baseArea.GetHexCell().coordinates);
    }
    public void AddArea(Area area)
    {
        if (area == null)
            return;
        if(top == null)
        {
            top = bottom = left = right = area;
        }else
        {
            Vector3 pos = area.gameObject.transform.parent.position;
            if (left.transform.position.x > pos.x)
            {
                left = area;
            } else if(right.transform.position.x < pos.x)
            {
                right = area;
            }
            if(bottom.transform.position.z > pos.z)
            {
                bottom = area;
            } else if(top.transform.position.z < pos.z)
            {
                top = area;
            }
        }
        areas.Add(area);
    }
    public void SetBaseArea(Area area)
    {
        baseArea = area;
        hexSpiral.setCoordinates(baseArea.GetHexCell().coordinates);
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
    public void AddEvent(MainEvent anEvent) {
        includedEvents.Add(anEvent);
    }
    public List<MainEvent> GetEvents()
    {
        return includedEvents;
    }
    public List<MainEvent> GetRevealedEvents()
    {
        return includedEvents.FindAll(anEvent => anEvent.isAppeared());
    }

    public void CalculateCenter()
    {
        Vector3[] positions = GetBorders();

        float[] xArr = new float[4];
        float[] zArr = new float[4];

        xArr[0] = positions[0].x;
        xArr[1] = positions[1].x;
        xArr[2] = positions[2].x;
        xArr[3] = positions[3].x;
        zArr[0] = positions[0].z;
        zArr[1] = positions[1].z;
        zArr[2] = positions[2].z;
        zArr[3] = positions[3].z;

        float maxX = Mathf.Max(xArr);
        float minX = Mathf.Min(xArr);
        float maxZ = Mathf.Max(zArr);
        float minZ = Mathf.Min(zArr);

        Vector3 focusPosition = new Vector3((maxX + minX) / 2, 150f, (maxZ + minZ) / 2);

        this.center = focusPosition;
    }

    public Vector3[] GetBorders()
    {
        Vector3[] positions = new Vector3[4];
        positions[0] = left.transform.position;
        positions[1] = right.transform.position;
        positions[2] = top.transform.position;
        positions[3] = bottom.transform.position;
        return positions;
    }

    public Vector3 GetCenter()
    {
        return center;
    }
}
