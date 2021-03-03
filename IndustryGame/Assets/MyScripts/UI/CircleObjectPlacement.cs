using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleObjectPlacement : MonoBehaviour
{
    [SerializeField]
    [Min(0)]
    private float radius = 100;
    private List<GameObject> objects;
    public void Clear()
    {
        if(objects != null)
        {
            foreach(GameObject eachObject in objects) {
                Destroy(eachObject);
            }
        }
    }
    public void Set(List<GameObject> objects)
    {
        Clear();
        this.objects = objects;
    }
    private void Update()
    {
        if (objects == null)
            return;
        int objectCount = objects.Count;
        for (int i = 0; i < objectCount; ++i)
        {
            float angle = 2 * Mathf.PI * i / objectCount;
            objects[i].transform.parent = HUDManager.instance.transform;
            objects[i].transform.position = Camera.main.WorldToScreenPoint(transform.position);
            objects[i].transform.position += new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
        }
    }
}
