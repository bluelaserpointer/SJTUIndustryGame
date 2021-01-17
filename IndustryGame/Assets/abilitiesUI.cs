using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class abilitiesUI : MonoBehaviour
{
    public Text abilityName;
    public Text abilityLevel;

    public void delete ()
    {
        Destroy(gameObject);
    }
}
