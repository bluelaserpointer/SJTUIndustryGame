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
    public Area left, right, bottom, top;
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
        GameObject parent = area.transform.parent.gameObject;
        // Debug.Log("Region " + regionId + " " + area.transform.position);
        // Debug.Log("Region " + regionId + " " + area.transform.TransformPoint(area.transform.position));
        // Debug.Log("Region " + regionId + " " + parent.transform.parent.TransformPoint(parent.transform.localPosition));


        Vector3 pos = area.gameObject.transform.parent.position;
        if(area == null)
            return;

        if(top == null)
        {
            // area.name = "Region " + regionId;
            // GameObject.Destroy(area.gameObject.transform.parent.gameObject);
            // Debug.Log("Region " + regionId + ": First init: " + area.transform.position);
            top = area;
            bottom = area;
            left = area;
            right = area;
        }else{
            if(left.transform.position.x > pos.x)
            {
                left = area;
                Debug.Log("Region " + regionId + ": Updating left");
            }
            if(right.transform.position.x < pos.x)
            {
                right = area;
                Debug.Log("Region " + regionId + ": Updating right");

            }
            if(bottom.transform.position.z > pos.z)
            {
                bottom = area;
                Debug.Log("Region " + regionId + ": Updating bottom");

            }
            if(top.transform.position.z < pos.z)
            {
                top = area;
                Debug.Log("Region " + regionId + ": Updating top");

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
}
