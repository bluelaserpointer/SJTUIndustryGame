using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class GlobalCanvas : MonoBehaviour
{
    public static Canvas canvas;
    private void Awake()
    {
        if(canvas == null)
            canvas = GetComponent<Canvas>();
    }
}
