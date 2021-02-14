using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LookAtMainCamera : MonoBehaviour
{
    void Update()
    {
        if(transform.gameObject.activeSelf)
        {
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (mainCamera == null)
                return;
            //transform.rotation = Quaternion.LookRotation(new Vector3(0, mainCamera.transform.position.y - transform.position.y, Mathf.Abs(mainCamera.transform.position.z - transform.position.z)));
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
