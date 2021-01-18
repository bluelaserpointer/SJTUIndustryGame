using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HireUI : MonoBehaviour
{
    public List<Specialist> specialists;
    public Ability ability;
    public GameObject HireProfile1;
    public GameObject HireProfile2;
    public GameObject HireProfile3;

    public Text AbilityText;
    //public Image AbilityPhoto;

    
    void Start()
    {
        AbilityText.text = AbilityDescription.GetAbilityDescription(ability);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHireList ()
    {
        specialists = SpecialistEmployList.getSpecialists(ability);
        HireProfile1.GetComponent<ProfileManage>().specialist = specialists[0];
        HireProfile2.GetComponent<ProfileManage>().specialist = specialists[1];
        HireProfile3.GetComponent<ProfileManage>().specialist = specialists[2];
    }


}
