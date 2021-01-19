using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpWindow : MonoBehaviour
{
    public string title;
    public string contents;

    public void show()
    {
        transform.gameObject.SetActive(true);
    }
}