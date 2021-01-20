using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class OrthographicCamera : MonoBehaviour
{
    private static OrthographicCamera instance;

    [SerializeField]
    public float cameraParam = 1.5f;
    public float transitionSpeed = 0.25f; 
    private Camera mainCamera;

    // Orthographic Sizes
    public float mouseOrthographicSize = 7f;
    public float keyOrthographicSize = 9f;
    public float maskOrthographicSize = 15f;
    private float currentSize;
    private float originalSize;


    private Transform originalTransform;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Quaternion orthographicRotation;

    private bool cameraFocus = false;
    private HexGrid hexGrid;
    private float orthographicRotationX;
    private Vector3 orthographicPosition;
    private HexCell currentHexCell;
    private Area currentArea;
    private int focusMask;

    // AreaDetails HUD
    public GameObject AreaDetailsHUD;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        originalTransform = transform;
        originalSize = currentSize = mainCamera.orthographicSize;

        currentPosition = originalPosition = originalTransform.position;
        currentRotation = originalRotation = originalTransform.rotation;
        orthographicRotation = currentRotation * Quaternion.Euler(-cameraParam, 0, 0);

        focusMask = LayerMask.GetMask("FocusMask");
    }
    void Update () {
        handleCameraFocus();
        AreaDetailsHUD.SetActive(cameraFocus);
    }


    private void handleFocusAreaAudio()
    {
        AreaWeatherSFXRandomPlayer.setArea(currentArea);

        List<List<Animal>> animals = currentArea.getSpeciesDangerTypes();
        int areaMostDangerType = 0;

        if(animals.Count > 0)
        {
            AreaAnimalSFXRandomPlayer.setAnimalList(animals);
            int dangerTypeNum = EnumHelper.GetMaxEnum<SpeciesDangerType>();
            for(int i = dangerTypeNum; i > 0; i--)
            {
                if(animals[i].Count > 0)
                {
                    areaMostDangerType = i;
                    break;
                }
            }
        }

        // if(areaMostDangerType > 0)
        //     AreaBGMRandomPlayer.SetDangerBgmList(areaMostDangerType);
        // else
        //     AreaBGMRandomPlayer.SetAreaBgmList(currentArea);
    }

    private void handleGlobalAudio()
    {
        AreaWeatherSFXRandomPlayer.Silence();
        // AreaBGMRandomPlayer.SetGlobalBgmList();
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
        if(mainCamera.orthographicSize <= maskOrthographicSize)
        {
            mainCamera.cullingMask &= ~(1 << 8); // 关闭层x
        }
        else
        {
            mainCamera.cullingMask |= (1 << 8);
        }

        handleCameraControl();
    }

    private void handleCameraControl()
    {
        if(cameraFocus)
        {
            // Exiting with esc
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentSize = originalSize;
                currentPosition = originalPosition;
                currentRotation = originalRotation;

                SetCameraFocus(false);
            }

            // Moving with WASD
            HexCell focusHexCell = null;
            if(Input.GetKeyDown(KeyCode.W))
            {
                focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.NW);
                if(focusHexCell == null)
                    focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.NE);
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.SE);
                if(focusHexCell == null)
                    focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.SW);
            }
            if(Input.GetKeyDown(KeyCode.A))
                focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.W);
            if(Input.GetKeyDown(KeyCode.D))
                focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.E);
            
            if(focusHexCell != null)
            {
                currentHexCell = focusHexCell;
                Vector3 focusPosition = focusHexCell.transform.position;
                focusOnHexCell(focusPosition, keyOrthographicSize);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                currentSize = mouseOrthographicSize;
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                currentSize = keyOrthographicSize;

        }else if (!IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
        {
            // Focusing with mouse
            Area pointingArea;
            Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                currentHexCell = hexGrid.GetCell(hit.point);
                pointingArea = currentHexCell.transform.GetComponent<Area>();
                Vector3 pointingAreaPosition = pointingArea.transform.position;

                focusOnHexCell(pointingAreaPosition, mouseOrthographicSize);
            }else{
                pointingArea = null;
            }
        }
    }

    public void focusOnHexCell(Vector3 focusPosition, float orthographicSize)
    {
        currentArea = currentHexCell.gameObject.GetComponent<Area>();

        SetCameraFocus(true);

        currentSize = orthographicSize;
        currentPosition = new Vector3(focusPosition.x, focusPosition.y + 12.8f, focusPosition.z - 14.5f);
        currentRotation = orthographicRotation;
    }

    private void SetCameraFocus(bool value)
    {
        if(!cameraFocus && !value)
            return;
        
        if(value)
            handleFocusAreaAudio();
        else
            handleGlobalAudio();
        cameraFocus = value;
    }
    public static Area GetMousePointingArea()
    {
        return instance != null ? instance.currentArea : null;
    }
}
