using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseWindow : MonoBehaviour
{
    private Button CloseButton;

    void Start()
    {
        CloseButton = gameObject.GetComponent<Button>();
        CloseButton.onClick.AddListener(() => WindowsManager.instance.ClearWindow());
    }
}
