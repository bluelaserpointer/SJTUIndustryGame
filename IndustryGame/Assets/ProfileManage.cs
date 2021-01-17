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

    public GameObject SingleAbility;
    public GameObject AbilityList;

    void Update ()
    {
        
    }

    public void UpdateProfile ()
    {
        //Avatar.sprite = specialist.specialistTemplate.icon;
        Name.text = specialist.name;
        Birthplace.text = specialist.birthplace;
        BirthDate.text = specialist.birthday;
        Speciality.text = specialist.speciality.ToString();
        Gender.text = specialist.specialistTemplate.jender.ToString();

        IDictionaryEnumerator enumerator = specialist.abilities.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GameObject clone = Instantiate(SingleAbility, AbilityList.transform, false);
            clone.GetComponent<abilitiesUI>().abilityName.text = enumerator.Key.ToString();
            clone.GetComponent<abilitiesUI>().abilityLevel.text = enumerator.Value.ToString();
        }
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
