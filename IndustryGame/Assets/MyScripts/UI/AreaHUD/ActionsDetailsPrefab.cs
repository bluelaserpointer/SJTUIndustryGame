using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsDetailsPrefab : MonoBehaviour
{
    public Text ActionNameText;

    public void RefreshUI(AreaAction areaAction)
    {
        ActionNameText.text = areaAction.name;
        Debug.Log("action Name: " + ActionNameText.text);
    }
}
