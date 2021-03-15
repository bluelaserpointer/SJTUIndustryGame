using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpecialistDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
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

        if(raycasted)
        {
            HexCell hexCell = Stage.GetHexGrid().GetCell(hit.point);
            if(hexCell != null)
            {
                Area area = hexCell.transform.GetComponentInChildren<Area>();
                if(area != null)
                {
                    if(area.region.regionId != -1)
                        area.ShowSpecialistActionButtons(GetComponentInParent<SingleBarSpecialist>().specialist);

                }else{
                }

                SpecialistBar.instance.RefreshList();
            }
                        
        }
    }
}
