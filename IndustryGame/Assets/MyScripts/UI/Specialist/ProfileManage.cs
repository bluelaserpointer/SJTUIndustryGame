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
    public bool HireMode = true;

    public GameObject SingleAbility;
    public GameObject AbilityList;

    private void OnDisable()
    {
        if (HireMode)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        clearAbilityList();
        UpdateProfile();
    }

    public void UpdateProfile()
    {
        if (specialist == null)
        {
            return;
        }
        gameObject.SetActive(true);
        Name.text = specialist.name;
        Birthplace.text = specialist.birthplace;
        BirthDate.text = specialist.birthday;
        Speciality.text = specialist.speciality.ToString();
        Gender.text = specialist.specialistTemplate.jender.ToString();
        Avatar.sprite = specialist.specialistTemplate.icon;
        if (HireMode)
        {
            HireCost.text = "$" + specialist.hireCost.ToString();
        }

        foreach (KeyValuePair<Ability, int> pair in specialist.abilities)
        {
            GameObject clone = Instantiate(SingleAbility, AbilityList.transform, false);
            clone.GetComponent<abilitiesUI>().abilityName.text = AbilityDescription.GetAbilityDescription(pair.Key);
            clone.GetComponent<abilitiesUI>().abilityLevel.text = pair.Value.ToString();
            InGameLog.AddLog(pair.Key.ToString() + ": " + pair.Value.ToString());
        }
    }

    public void clearAbilityList()
    {
        abilitiesUI[] list = gameObject.GetComponentsInChildren<abilitiesUI>();
        for (int i = 0; i < list.Length; i++)
        {
            list[i].delete();
        }
    }

    public void HireSpecialist()
    {
        SpecialistEmployList.hireSpecialist(specialist);
        specialist = null;
        gameObject.SetActive(false);
        SpecialistBar.instance.RefreshList();

    }
}
