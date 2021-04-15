using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustMove : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 initialPosition;
    void Start()
    {
        initialPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = initialPosition + new Vector3(Random.Range(-1, 1),0,Random.Range(-1,1));
    }
}
