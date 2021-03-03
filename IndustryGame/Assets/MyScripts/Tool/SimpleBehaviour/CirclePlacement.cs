using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CirclePlacement : MonoBehaviour
{
    [SerializeField]
    [Min(0)]
    private float radius = 100;
    public Transform centerTransform;
    [SerializeField]
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
    public void SetObjects(List<GameObject> objects)
    {
        Clear();
        this.objects = objects;
        objects.ForEach(eachObject => eachObject.transform.parent = transform);
    }
    public void SetObjects(Transform centerTransform, List<GameObject> objects)
    {
        this.centerTransform = centerTransform;
        SetObjects(objects);
    }
    private void Update()
    {
        if (objects == null)
            return;
        int objectCount = objects.Count;
        for (int i = 0; i < objectCount; ++i)
        {
            float angle = 2 * Mathf.PI * i / objectCount;
            objects[i].transform.position = Camera.main.WorldToScreenPoint(centerTransform.position) + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
        }
    }
}
