using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManage : MonoBehaviour
{
    //[SerializeField]
    public Specialist specialist;
    public Image Avatar;
    public Text Name;
    public Text Birthplace;
    public Text BirthDate;
    public Text Speciality;
    public Text Gender;
    public Text HireCost;
    public bool Hired = false;

    public GameObject SingleAbility;
    public GameObject AbilityList;

    void Update ()
    {
        clearAbilityList();
        UpdateProfile();
    }

    public void UpdateProfile ()
    {
        //Avatar.sprite = specialist.specialistTemplate.icon;
        Name.text = specialist.name;
        Birthplace.text = specialist.birthplace;
        BirthDate.text = specialist.birthday;
        Speciality.text = specialist.speciality.ToString();
        Gender.text = specialist.specialistTemplate.jender.ToString();
        //if (!Hired)
        //{
        //    HireCost.text = specialist.hireCost.ToString();
        //}

        foreach (KeyValuePair<Ability, int> pair in specialist.abilities)
        {
            GameObject clone = Instantiate(SingleAbility, AbilityList.transform, false);
            clone.GetComponent<abilitiesUI>().abilityName.text = AbilityDescription.GetAbilityDescription(pair.Key);
            clone.GetComponent<abilitiesUI>().abilityLevel.text = pair.Value.ToString();
            InGameLog.AddLog(pair.Key.ToString() + ": " + pair.Value.ToString());
        }

        //IDictionaryEnumerator enumerator = specialist.abilities.GetEnumerator();
        //while (enumerator.MoveNext())
        //{
            
        //}
    }

    public void clearAbilityList ()
    {
        abilitiesUI[] list = gameObject.GetComponentsInChildren<abilitiesUI>();
        for (int i = 0 ; i < list.Length ; i++)
        {
            list[i].delete();
        }
    }
}
