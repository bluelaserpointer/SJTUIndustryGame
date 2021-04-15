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
    public Slider LevelSlider;

    public ProfileManage profile;

    void Update()
    {
        Avatar.sprite = specialist.specialistTemplate.icon;
        Name.text = specialist.name;
        Speciality.text = specialist.speciality.ToString() == "Indoor" ? "室内型" : "户外型";
        LevelSlider.value = specialist.GetExpRate();
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    public void DisplayProfile()
    {
        profile = GameObject.FindGameObjectWithTag("MyProfile").GetComponent<ProfileManage>();
        profile.specialist = this.specialist;
        profile.clearAbilityList();
        profile.UpdateProfile();
    }


}
