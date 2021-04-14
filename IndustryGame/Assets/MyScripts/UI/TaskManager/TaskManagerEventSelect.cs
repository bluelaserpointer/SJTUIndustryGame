using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManagerEventSelect : MonoBehaviour
{
    public MainEvent mainEvent;

    public Text EventName;

    public Image BackgroundImage;

    private Button button;

    private Slider EventProgress;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        EventProgress = gameObject.GetComponentInChildren<Slider>();
        EventProgress.maxValue = mainEvent.eventStages.Count;
        // button.onClick.AddListener(() => FilterPanel.instance.focusHelperForEvent.SelectImage(BackgroundImage));
        // button.onClick.AddListener(() => FilterPanel.GenerateAnimalSelectionPanel(mainEvent));
    }

    void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        EventName.text = mainEvent.name;
        EventProgress.value = mainEvent.GetRevealedEventStages().Count;

    }
}
