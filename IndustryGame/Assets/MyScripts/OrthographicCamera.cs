using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrthographicCamera : MonoBehaviour
{
    [SerializeField]
    public float cameraParam = 1.5f;
    public float transitionSpeed = 0.25f; 
    private Camera mainCamera;
    private float currentSize;
    private float originalSize;
    private float orthographicSize = 6.5f;

    // private Vector3 originalPosition;
    private Transform originalTransform;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // private Quaternion originalRotation;
    private Quaternion orthographicRotation;

    private bool cameraFocus = false;
    private HexGrid hexGrid;
    

    private float orthographicRotationX;
    private Vector3 orthographicPosition;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        originalTransform = transform;
        originalSize = currentSize = mainCamera.orthographicSize;

        currentPosition = originalPosition = originalTransform.position;
        currentRotation = originalRotation = originalTransform.rotation;
        orthographicRotation = currentRotation * Quaternion.Euler(-cameraParam, 0, 0);
    }
    void Update () {
        handleCameraFocus();
    }

    private void FixedUpdate() {
        //Lerp position
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, currentSize, Time.deltaTime * transitionSpeed * 10);

        transform.position = Vector3.Lerp(transform.position, currentPosition, transitionSpeed);
        
        Vector3 currentAngle = new Vector3 (
        Mathf.LerpAngle(transform.rotation.eulerAngles.x, currentRotation.eulerAngles.x,  Time.deltaTime * transitionSpeed),
        Mathf.LerpAngle(transform.rotation.eulerAngles.y, currentRotation.eulerAngles.y,  Time.deltaTime * transitionSpeed),
        Mathf.LerpAngle(transform.rotation.eulerAngles.z, currentRotation.eulerAngles.z,  Time.deltaTime * transitionSpeed));
        transform.eulerAngles = currentAngle;    
    }

    public void SetHexGrid(HexGrid hexGrid)
    {
        this.hexGrid = hexGrid;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    private void handleCameraFocus() {
        Area pointingArea = Stage.GetMousePointingArea();

        if (!IsPointerOverUIObject() && Input.GetMouseButtonDown(0) && cameraFocus == false)
        {
            Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                HexCell hexCell = hexGrid.GetCell(hit.point);
                pointingArea = hexCell.transform.GetComponent<Area>();
                Vector3 pointingAreaPosition = pointingArea.transform.position;

                cameraFocus = true;

                currentSize = orthographicSize;
                currentPosition = new Vector3(pointingAreaPosition.x, pointingAreaPosition.y + 12.8f, pointingAreaPosition.z - 14.5f);
                currentRotation = orthographicRotation;
            }else{
                pointingArea = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && cameraFocus == true)
        {
            currentSize = originalSize;
            currentPosition = originalPosition;
            currentRotation = originalRotation;

            cameraFocus = false;
        }
    }
}
