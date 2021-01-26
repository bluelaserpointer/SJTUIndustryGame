using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    public int previousRegionId = -2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.transform.InverseTransformPoint(transform.position).z));
        }
    }
}
