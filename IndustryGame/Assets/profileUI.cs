using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class profileUI : MonoBehaviour
{
    [SerializeField]
    public Specialist specialist;

    private Sprite Avatar;
    private Text Name;
    private Text Birthplace;
    private Text BirthDate;
    private Text Speciality;
    private Text Gender;

    private GameObject SingleAbility;
    private GameObject AbilityList;

    void Update()
    {
        Avatar = specialist.specialistTemplate.icon;
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
