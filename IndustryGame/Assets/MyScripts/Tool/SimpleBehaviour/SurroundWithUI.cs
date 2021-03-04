using System.Collections.Generic;
using UnityEngine;

public class SurroundWithUI : MonoBehaviour
{
    public bool useMainCamera = true;
    public Camera defaultCamera;
    public float radius;
    public float deltaAngle;
    private readonly List<GameObject> surrounders = new List<GameObject>();
    public void AddSurrounders(Canvas parentCanvas, params GameObject[] surrounders)
    {
        AddSurrounders(parentCanvas, surrounders as IEnumerable<GameObject>);
    }
    public void AddSurrounders(Canvas parentCanvas, IEnumerable<GameObject> surrounders)
    {
        foreach (GameObject surrounder in surrounders)
        {
            surrounder.transform.parent = parentCanvas.transform;
            this.surrounders.Add(surrounder);
        }
    }
    public void DestroySurrounders()
    {
        surrounders.ForEach(eachSurrounder => Destroy(eachSurrounder));
        surrounders.Clear();
    }
    public void DestroySurrounder(GameObject surrounder)
    {
        if(surrounders.Remove(surrounder))
            Destroy(surrounder);
    }
    private void Update()
    {
        int surrounderCount = surrounders.Count;
        for (int i = 0; i < surrounderCount; ++i)
        {
            float angle = deltaAngle + 2 * Mathf.PI * i / surrounderCount;
            surrounders[i].transform.position = (useMainCamera ? Camera.main : defaultCamera).WorldToScreenPoint(transform.position) + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
        }
    }
}
