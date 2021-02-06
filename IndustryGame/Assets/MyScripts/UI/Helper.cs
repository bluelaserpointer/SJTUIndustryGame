using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A helper class for UI
public class Helper : MonoBehaviour
{
    public static Helper instance;

    public static void ClearList(List<GameObject> list)
    {
        if (list != null)
        {
            foreach (GameObject gameObject in list)
            {
                Destroy(gameObject);
            }
            list.Clear();
        }
    }

    public static void RefreshRegionDropdown(Dropdown dropdown)
    {
        dropdown.options.Clear();

        if (Stage.GetRegions().Count > 0)
        {
            foreach (Region region in Stage.GetRegions())
            {
                Dropdown.OptionData tempData = new Dropdown.OptionData();
                tempData.text = region.name;
                //tempData.image = CurrentArea.GetEnabledActions()[i].actionName;
                dropdown.options.Add(tempData);
                //InGameLog.AddLog(Stage.GetSpecialists()[i].name);
            }
            dropdown.captionText.text = Stage.GetRegions()[dropdown.value].name;
        }
    }
}
