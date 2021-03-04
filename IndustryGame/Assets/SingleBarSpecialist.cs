using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleBarSpecialist : MonoBehaviour
{
    [HideInInspector]
    public Specialist specialist;

    [Header("专家名字")]
    public Text specialistName;
    [Header("专家头像")]
    public Image avatar;

    public void RefreshUI(Specialist specialist)
    {
        this.specialist = specialist;
        specialistName.text = specialist.name;
        avatar.sprite = specialist.specialistTemplate.icon;
    }
}
