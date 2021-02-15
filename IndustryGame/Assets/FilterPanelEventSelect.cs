using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanelEventSelect : MonoBehaviour
{
    public MainEvent mainEvent;

    public Text EventName;

    private Button button;
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        // button.onClick.AddListener(() => FilterPanel.RefreshEventsDueToRegionSelection(region));
    }

    void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        EventName.text = mainEvent.name;
    }
}
