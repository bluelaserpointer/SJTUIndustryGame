using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentReportUI : MonoBehaviour
{
    [HideInInspector]
    public Region region;


    [Header("地域名称")]
    public Text regionNameText;

    [Header("指标选择Dropdown")]
    public Dropdown environmentStatSelection;
    [Header("生成图像位置")]
    public GameObject ImageGeneratePosition;

    [Header("HexagonImagePrefab")]
    public GameObject HexagonImagePrefab;
    [Header("没有被影响到的地区颜色")]
    public Color UnaffectedAreaColor;

    private List<string> dropDownValue = new List<string>();
    private List<GameObject> GeneratedHexagons = new List<GameObject>();
    // [Header("环境指标说明")]
    // public Text descriptionText;

    // [Header("生成列表的位置")]
    // public GameObject BuffsListPosition;
    // [Header("单个显示的Buff的 UI Prefab")]
    // public GameObject SingleBuffsPrefab;

    // void Update()
    // {
    //     RefreshUI();
    // }


    public void RefreshUI()
    {
        if (region == null)
        {
            InGameLog.AddLog("Error: Region is not assigned");
            return;
        }

        regionNameText.text = region.name;

        Helper.ClearList(GeneratedHexagons);

        foreach (Area area in region.GetAreas())
        {
            if (dropDownValue.Count != 0)
            {
                ImageGeneratePosition.transform.localScale = new Vector3(1, 1, 1);
                // Debug.Log("area EnvironmentStat value: " + area.GetEnviromentStatWithString(dropDownValue[environmentStatSelection.value]));
                // Debug.Log("area x value: " + area.regionPositionX + "area y value: " + area.regionPositionY);
                GameObject clone = Instantiate(HexagonImagePrefab, ImageGeneratePosition.transform, false);
                clone.transform.localPosition = new Vector3(area.regionPositionX - (ImageGeneratePosition.GetComponent<RectTransform>().rect.width / 3), area.regionPositionY + (ImageGeneratePosition.GetComponent<RectTransform>().rect.height / 3), 0);

                if (area.GetEnviromentStatWithString(dropDownValue[environmentStatSelection.value]) == 0.0f)
                {
                    clone.GetComponent<Image>().color = UnaffectedAreaColor;
                }
                else
                {
                    clone.GetComponent<Image>().color = new Color(clone.GetComponent<Image>().color.r, clone.GetComponent<Image>().color.g, clone.GetComponent<Image>().color.b, area.GetEnviromentStatWithString(dropDownValue[environmentStatSelection.value]));
                }
                GeneratedHexagons.Add(clone);
                ImageGeneratePosition.transform.localScale = new Vector3(2, 2, 1);
            }
        }
    }

    void Start()
    {
        Helper.RefreshEnvironmentStatDropDown(environmentStatSelection, region, dropDownValue);
    }

    void Update()
    {
        Helper.RefreshEnvironmentStatDropDown(environmentStatSelection, region, dropDownValue);
        RefreshUI();

    }

}
