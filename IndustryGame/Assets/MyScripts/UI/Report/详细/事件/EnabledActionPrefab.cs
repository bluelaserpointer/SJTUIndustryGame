using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnabledActionPrefab : MonoBehaviour
{
    [Header("显示措施名")]
    public Text ActionName;

    public void Generate(AreaAction areaAction)
    {
        ActionName.text = areaAction.actionName;
    }
}
