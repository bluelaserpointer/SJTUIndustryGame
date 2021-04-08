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
                    {
                        //if (hexCell == null) Debug.Log("Hexcell is null, can't move expert");
                        //if (GetComponentInParent<SingleBarSpecialist>().specialist.Area.GetHexCell() == null) Debug.Log("specialist cell is null,can't remove expert");
                        /*if (hexCell != GetComponentInParent<SingleBarSpecialist>().specialist.Area.GetHexCell())//需要转移区域
                        {
                            MoveExpert(GetComponentInParent<SingleBarSpecialist>().specialist.Area.GetHexCell(), hexCell);
                        }*/
                        area.ShowSpecialistActionButtons(GetComponentInParent<SingleBarSpecialist>().specialist);
                    }
                        

                }else{
                }

                SpecialistBar.instance.RefreshList();
            }
                        
        }
    }

}
