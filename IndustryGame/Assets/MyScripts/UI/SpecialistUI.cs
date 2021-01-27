using UnityEngine;


public class SpecialistUI : MonoBehaviour
{
    public GameObject SpecialistList;
    public GameObject SingleSpecialistPrefab;

    void Start ()
    {
        InstantiateSpecialistList();
    }

    void Update()
    {
    }

    private void OnEnable ()
    {
        InstantiateSpecialistList();
        InGameLog.AddLog("In SpecialistUI OnEnable");

    }

    void InstantiateSpecialistList ()
    {
        foreach (Specialist specialist in Stage.GetSpecialists())
        {
            GameObject clone;
            clone = Instantiate(SingleSpecialistPrefab, SpecialistList.transform, false);
            clone.GetComponent<SingleSpecialist>().specialist = specialist;
            InGameLog.AddLog("Ins : " + specialist.name);
        }
    }

   
}
