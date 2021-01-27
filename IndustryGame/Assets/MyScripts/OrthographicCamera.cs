using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class OrthographicCamera : MonoBehaviour
{
    private static OrthographicCamera instance;
    private Camera mainCamera;

    [Header("Orthographic Sizes")]
    public float mouseAreaOrthoSize = 7f;
    public float keyAreaOrthoSize = 9f;
    public float maskAreaOrthoSize = 15f;    
    public float worldOrthoSize;

    [Header("Speed for different lerps")]
    public float sizeSpeed;
    public float positionSpeed;
    public float rotationSpeed;

    [Header("World Transform")]
    public Vector3 worldPosition;
    public Quaternion worldRotation;

    private float regionSize;
    private float currentSize;
    private Transform worldTransform;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Vector3 regionPosition;
    private Quaternion regionRotation;
    [Header("Area Transform")]
    public float orthoAreaRotationX;
    public Vector3 areaPositionOffset;
    private Quaternion orthoAreaRotation;
    [Header("Region Transform")]
    public float orthoRegionRotationX;
    
    private Quaternion orthoRegionRotation;


    private bool regionFocus = false;
    private bool areaFocus = false;
    private HexGrid hexGrid;
    private HexCell currentHexCell;
    private Area currentArea;
    private Region currentRegion;
    private int focusMask;

    [Header("AreaDetails HUD")]
    public GameObject AreaDetailsHUD;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // MainCamera Initial Positioning
        mainCamera = Camera.main;
        Region region = Stage.GetRegions().Find(eachRegion => eachRegion.GetRegionId() == -1);
        mainCamera.transform.position = region.GetCenter();
        mainCamera.orthographicSize = worldOrthoSize;

        worldTransform = transform;

        currentSize = mainCamera.orthographicSize;
        currentPosition = worldPosition = worldTransform.position;
        currentRotation = worldRotation = worldTransform.rotation;

        orthoAreaRotation = Quaternion.Euler(45, orthoRegionRotation.y, orthoRegionRotation.z);
        orthoRegionRotation = Quaternion.Euler(90, orthoRegionRotation.y, orthoRegionRotation.z);

        focusMask = LayerMask.GetMask("FocusMask");
    }
    void Update () {
        handleCameraFocus();
        AreaDetailsHUD.SetActive(areaFocus);
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

        if(areaMostDangerType > 0)
            AreaBGMRandomPlayer.SetDangerBgmList(areaMostDangerType);
        else
            AreaBGMRandomPlayer.SetAreaBgmList(currentArea);
    }

    private void handleGlobalAudio()
    {
        AreaWeatherSFXRandomPlayer.Silence();
        AreaBGMRandomPlayer.SetGlobalBgmList();
    }

    private void FixedUpdate() {
        //Lerp position

        // if(regionFocus && !areaFocus)
        // {
        //     MeshRenderer renderer = currentRegion.bottom.transform.parent.parent.GetComponentInChildren<MeshRenderer>();
        //     if(!renderer.isVisible)
        //     {
        //         currentSize = GetRequiredOrthoSize(mainCamera, positions);
        //         // currentPosition
        //     }
        // }

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, currentSize, Time.deltaTime * sizeSpeed);

        transform.position = Vector3.Lerp(transform.position, currentPosition, positionSpeed);
        
        Vector3 currentAngle = new Vector3 (
        Mathf.LerpAngle(transform.rotation.eulerAngles.x, currentRotation.eulerAngles.x, rotationSpeed),
        Mathf.LerpAngle(transform.rotation.eulerAngles.y, currentRotation.eulerAngles.y, rotationSpeed),
        Mathf.LerpAngle(transform.rotation.eulerAngles.z, currentRotation.eulerAngles.z, rotationSpeed));
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
        if(mainCamera.orthographicSize <= maskAreaOrthoSize)
        {
            mainCamera.cullingMask &= ~(1 << 8); // 关闭层x
        }
        else
        {
            mainCamera.cullingMask |= (1 << 8);
        }

        handleRegionFocus();
    }

    private void handleRegionFocus()
    {
            bool areaToRegionFlag = false;
            handleAreaFocus(ref areaToRegionFlag);
            if(areaToRegionFlag == true)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetCurrent(worldOrthoSize, worldPosition, worldRotation);

                SetRegionFocus(false);
            }

            if(!IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
            {
                Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(inputRay, out hit))
                {
                    focusOnRegion(hexGrid.GetCell(hit.point));
                }
            }
    }

    private void handleAreaFocus(ref bool areaToRegionFlag)
    {
        if(areaFocus)
        {
            // Exiting with esc
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetCurrent(regionSize, regionPosition, regionRotation);

                SetAreaFocus(false);
                areaToRegionFlag = true;
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
                focusOnArea(focusHexCell, keyAreaOrthoSize);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                currentSize = mouseAreaOrthoSize;
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                currentSize = keyAreaOrthoSize;

        }else if (!IsPointerOverUIObject() && Input.GetMouseButtonDown(0) && !HUDManager.CheckOpenWindow() && regionFocus)
        {
            // Focusing with mouse
            Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                focusOnArea(hexGrid.GetCell(hit.point), mouseAreaOrthoSize);
            }
        }
    }

    public void focusOnArea(HexCell hexCell, float OrthoSize)
    {
        Area area = hexCell.transform.GetComponentInChildren<Area>();

        if(currentRegion.GetRegionId() != area.region.GetRegionId())
            return;

        currentHexCell = hexCell;
        currentArea = area;

        Vector3 focusPosition = hexCell.transform.position;
        SetCurrent(OrthoSize, new Vector3(focusPosition.x + areaPositionOffset.x, focusPosition.y + areaPositionOffset.y, focusPosition.z + areaPositionOffset.z), orthoAreaRotation);
        // SetCurrent(OrthoSize, new Vector3(focusPosition.x, focusPosition.y + 12.8f, focusPosition.z - 14.5f), orthoAreaRotation);

        SetAreaFocus(true);
    }

    public void focusOnRegion(HexCell hexCell)
    {
        Area area = hexCell.GetComponentInChildren<Area>();
        Region region = area.region;

        if(region.GetRegionId() == -1 || areaFocus)
            return;

        currentRegion = region;
        
        Vector3 focusPosition = region.GetCenter();
        Vector3[] positions = region.GetBorders();

        SetCurrent(GetRequiredOrthoSize(mainCamera, positions), focusPosition, orthoRegionRotation);

        regionSize = currentSize;
        regionPosition = currentPosition;
        regionRotation = currentRotation;

        SetRegionFocus(true);
    }

    public void SetCurrent(float currSize, Vector3 currPos, Quaternion currRot)
    {
        currentSize = currSize;
        currentPosition = currPos;
        currentRotation = currRot;
    }

    public float GetRequiredOrthoSize(Camera orthographicCamera, params Vector3[] positionBounds)
    {

 
        // Stored width and height are just your Screen.Width and Screen.Height
        float aspectRatio = (float)16 / (float)9;
 
        // 1: If you have an orthographic size of 5, the viewport will contain exactly 10          
        // units of world space
        float yUnits = orthographicCamera.orthographicSize * 2.0f;
 
        // 2: horizontal size is based on units * aspect ratio
        float xUnits = yUnits * aspectRatio;
 
        // 3: find the min and maximum to required based on world points
        float maxViewPortPoint = float.MinValue;
        float minViewPortPoint = float.MaxValue;
 
        for (int index = 0; index < positionBounds.Length; ++index)
        {
            float val = orthographicCamera.WorldToViewportPoint(positionBounds[index]).x;
 
            if (val > maxViewPortPoint)
            {
                maxViewPortPoint = val;
            }
 
            if (val < minViewPortPoint)
            {
                minViewPortPoint = val;
            }
        }
 
        // 4: calculate the viewport size based on these outer values
        float viewportSize = maxViewPortPoint - minViewPortPoint;
 
        // 5: Convert to actual X width. This can now be considered as Viewport normalised        
        // to 1 length
        float newXUnits = viewportSize * xUnits;
 
        // 6: Get the Y Units which match these X Units                                            
        // -> Reverse xUnits = yUnits * aspectratio
        float newYUnits = newXUnits / aspectRatio;
 
        // 7: Divide by 2 to get the actual ortho size -> again reverse of getting YUnits          
        // using ortho size
        float newOrtho = newYUnits / 2.0f;
 
        // 8: Apply
        return newOrtho + 100;
    }
    private void SetAreaFocus(bool value)
    {
        if(!areaFocus && !value)
            return;
        
        if(value)
            handleFocusAreaAudio();
        else
            handleGlobalAudio();

        areaFocus = value;
    }

    public void SetRegionFocus(bool value)
    {
        regionFocus = value;
    }

    public static Area GetMousePointingArea()
    {
        return instance != null ? instance.currentArea : null;
    }

}
