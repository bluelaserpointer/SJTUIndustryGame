using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleSpecialist : MonoBehaviour
{
    public Specialist specialist;
    public Image Avatar;
    public Text Name;
    public Text Speciality;


    void Update()
    {
        Avatar.sprite = specialist.specialistTemplate.icon;
        Name.text = specialist.name;
        Speciality.text = specialist.speciality.ToString();
    }

    private void OnDisable ()
    {
        Destroy(gameObject);
    }


}
