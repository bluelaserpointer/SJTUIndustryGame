using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsDetailsPrefab : MonoBehaviour
{
    private Building building;
    public Text buildingName;

    public void RefreshUI(Building building)
    {
        this.building = building;
        buildingName.text = building.info.buildingName;
    }

}
