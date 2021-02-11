using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleEnvironmentPrefab : MonoBehaviour
{
    [Header("显示环境的名称的Text")]
    public Text EnvironmentName;

    public void RefreshUI(Animal.EnvironmentPreference environment)
    {
        EnvironmentName.text = environment.environment.ToString();
    }
}
