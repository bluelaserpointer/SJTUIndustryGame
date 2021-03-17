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

    [Header("专家正在工作图标")]
    public GameObject working;

    

    public void RefreshUI(Specialist specialist)
    {
        this.specialist = specialist;
        specialistName.text = specialist.name;
        avatar.sprite = specialist.specialistTemplate.icon;
        if(specialist.HasAction){
            working.SetActive(true);
        }
        else{
            working.SetActive(false);
        }
    }

    
}
