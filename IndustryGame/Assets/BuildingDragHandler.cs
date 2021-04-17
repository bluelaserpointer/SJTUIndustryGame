using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool raycasted = Physics.Raycast(inputRay, out hit);
        BuildingsBar.instance.RefreshList();
        
        // if(IsPointerOverUIObject())
        //     return;

        if(raycasted)
        {
            HexCell hexCell = Stage.GetHexGrid().GetCell(hit.point);
            if(hexCell != null)
            {
                Area area = hexCell.transform.GetComponentInChildren<Area>();
                if(area != null)
                {
                    if(area.region.regionId != -1)
                        area.ShowConstructionButtons(GetComponentInParent<SingleBarBuildings>().buildingInfo);

                }else{
                }

            }
                        
        }
    }
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}
