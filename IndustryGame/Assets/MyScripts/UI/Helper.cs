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
            Dropdown.OptionData alltempData = new Dropdown.OptionData();
            alltempData.text = "全部";
            dropdown.options.Add(alltempData);
            foreach (Region region in Stage.GetRegions())
            {
                if (region.IsOcean) continue;
                Dropdown.OptionData tempData = new Dropdown.OptionData();
                tempData.text = region.name;
                //tempData.image = CurrentArea.GetEnabledActions()[i].actionName;
                dropdown.options.Add(tempData);
                //InGameLog.AddLog(Stage.GetSpecialists()[i].name);
            }
            if (dropdown.options[dropdown.value].text == "全部")
            {
                dropdown.captionText.text = "全部";
            }
            else
            {
                dropdown.captionText.text = Stage.GetRegions()[dropdown.value].name;
            }
        }
    }

    public static void RefreshEnvironmentStatDropDown(Dropdown dropdown, Region region, List<string> dropDownValue)
    {
        if (dropdown == null || dropDownValue == null || region == null)
        {
            Debug.Log("传参错误");
            return;
        }
        if (region.GetEvents().Count < 1)
        {
            Debug.Log("Error: Region Has no event to show environment report!");
            return;
        }
        else
        {
            dropdown.options.Clear();
            dropDownValue.Clear();
            // 有Event能够显示EnvironmentStat指标，要获取有哪些指标
            foreach (MainEvent mainEvent in region.GetEvents())
            {
                // Debug.Log("事件: " + mainEvent.name);
                if (mainEvent.GetRevealedStagesRelatedToEnvironment().Count < 1)
                {
                    // 现在还没有跟环境相关的EventStage
                    Debug.Log("Region: " + region.name + "中的MainEvent: '" + mainEvent.name + "'事件还没有跟环境相关的EventStage");
                }
                else
                {
                    foreach (EventStage eventStage in mainEvent.GetRevealedStagesRelatedToEnvironment())
                    {
                        // Debug.Log("Event Stage: " + eventStage.name + "    Related EnvironmentStat Name: " + eventStage.relatedEnvironmentStat.statName);
                        if (!dropDownValue.Contains(eventStage.relatedEnvironmentStat.statName))
                        {
                            // Debug.Log("Revealed EventStage: " + eventStage.name + "   statName: " + eventStage.relatedEnvironmentStat.statName);
                            Dropdown.OptionData tempData = new Dropdown.OptionData();
                            tempData.text = eventStage.relatedEnvironmentStat.statName;
                            dropdown.options.Add(tempData);
                            dropDownValue.Add(eventStage.relatedEnvironmentStat.statName);
                            //tempData.image = CurrentArea.GetEnabledActions()[i].actionName;
                            //InGameLog.AddLog(Stage.GetSpecialists()[i].name);
                        }
                    }
                }
            }
            // foreach (Dropdown.OptionData t in dropdown.options)
            // {
            //     Debug.Log("环境指标: " + t.text);
            // }
            // Debug.Log("Count: " + dropDownValue.Count);
            if (dropDownValue.Count > 0)
            {
                dropdown.captionText.text = dropDownValue[dropdown.value];
            }
        }
    }
}
