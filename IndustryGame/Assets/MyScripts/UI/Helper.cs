using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A helper class for UI
public class Helper : MonoBehaviour
{
    public static Helper instance;

    public static void ClearList(List<GameObject> list)
    {
        if (list != null)
        {
            foreach (GameObject gameObject in list)
            {
                Destroy(gameObject);
            }
            list.Clear();
        }
    }
}
