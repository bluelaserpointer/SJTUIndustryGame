using UnityEngine;

[DisallowMultipleComponent]
public class LookAtMainCamera : MonoBehaviour
{
    [SerializeField] private bool iso;
    void Update()
    {
        if(transform.gameObject.activeSelf)
        {
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (mainCamera == null)
                return;
            if(iso)
               transform.rotation = mainCamera.transform.rotation;
            else
               transform.rotation = Quaternion.LookRotation(new Vector3(0, mainCamera.transform.position.y - transform.position.y, Mathf.Abs(mainCamera.transform.position.z - transform.position.z)));

        }
    }
}
