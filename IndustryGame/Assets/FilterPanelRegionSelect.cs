using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanelRegionSelect : MonoBehaviour
{
    public Region region;
    [Header("显示Region名称")]
    public Text RegionName;
    public Image BackgroundImage;

    public GameObject EventCountObject;
    public Text EventCount;

    private Button button;
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() => FilterPanel.RefreshEventsDueToRegionSelection(region));
        button.onClick.AddListener(() => FilterPanel.instance.focusHelperForRegion.SelectImage(BackgroundImage, RegionName));
        EventCountObject.SetActive(false);
    }

    void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        RegionName.text = region.name;
        if (region.GetRevealedEvents().Count > 0)
        {
            EventCountObject.SetActive(true);
            EventCount.text = region.GetRevealedEvents().Count.ToString();
        }
        else
        {
            EventCountObject.SetActive(false);
        }
    }


}
