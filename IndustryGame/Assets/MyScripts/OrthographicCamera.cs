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
    public float mouseAreaOrthographicSize = 7f;
    public float keyAreaOrthographicSize = 9f;
    public float maskAreaOrthographicSize = 15f;
    private float currentSize;
    private float regionSize;
    private float worldSize;


    private Transform worldTransform;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private Vector3 regionPosition;
    private Quaternion regionRotation;
    private Vector3 worldPosition;
    private Quaternion worldRotation;
    private Quaternion orthographicAreaRotation;
    private Quaternion orthographicRegionRotation;


    private bool regionFocus = false;
    private bool areaFocus = false;
    private HexGrid hexGrid;
    private float orthographicAreaRotationX;
    private float orthographicRegionRotationX;
    private Vector3 orthographicAreaPosition;
    private Vector3 orthographicRegionPosition;

    private HexCell currentHexCell;
    private Area currentArea;
    private Region currentRegion;
    private int focusMask;
    private Vector3[] positions;

    public enum FocusStage{
        World,
        Region,
        Area
    }
    private FocusStage focusStage = FocusStage.World;

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
        worldTransform = transform;
        worldSize = currentSize = mainCamera.orthographicSize;

        currentPosition = worldPosition = worldTransform.position;
        currentRotation = worldRotation = worldTransform.rotation;

        orthographicAreaRotation = Quaternion.Euler(45, orthographicRegionRotation.y, orthographicRegionRotation.z);
        orthographicRegionRotation = Quaternion.Euler(90, orthographicRegionRotation.y, orthographicRegionRotation.z);

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

        if(regionFocus && !areaFocus)
        {
            MeshRenderer renderer = currentRegion.bottom.transform.parent.parent.GetComponentInChildren<MeshRenderer>();
            if(!renderer.isVisible)
            {
                currentSize = GetRequiredOrthographicSize(mainCamera, positions);
                // currentPosition
            }
        }

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, currentSize, Time.deltaTime * transitionSpeed * 10);

        transform.position = Vector3.Lerp(transform.position, currentPosition, transitionSpeed);
        
        Vector3 currentAngle = new Vector3 (
        Mathf.LerpAngle(transform.rotation.eulerAngles.x, currentRotation.eulerAngles.x,  Time.deltaTime * transitionSpeed * 4),
        Mathf.LerpAngle(transform.rotation.eulerAngles.y, currentRotation.eulerAngles.y,  Time.deltaTime * transitionSpeed * 4),
        Mathf.LerpAngle(transform.rotation.eulerAngles.z, currentRotation.eulerAngles.z,  Time.deltaTime * transitionSpeed * 4));
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
        if(mainCamera.orthographicSize <= maskAreaOrthographicSize)
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
        if(regionFocus)
        {
            // Keyboard control for region selection

            bool areaToRegionFlag = false;
            handleAreaFocus(ref areaToRegionFlag);
            if(areaToRegionFlag == true)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentSize = worldSize;
                currentPosition = worldPosition;
                currentRotation = worldRotation;

                SetRegionFocus(false);
            }


        }else if(!IsPointerOverUIObject() && Input.GetMouseButtonDown(0) && !regionFocus){
            Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                focusOnRegion(hexGrid.GetCell(hit.point), mouseAreaOrthographicSize);
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
                currentSize = regionSize;
                currentPosition = regionPosition;
                currentRotation = regionRotation;

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
                focusOnArea(focusHexCell, keyAreaOrthographicSize);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                currentSize = mouseAreaOrthographicSize;
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                currentSize = keyAreaOrthographicSize;

        }else if (!IsPointerOverUIObject() && Input.GetMouseButtonDown(0) && !HUDManager.CheckOpenWindow() && regionFocus)
        {
            // Focusing with mouse
            Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                focusOnArea(hexGrid.GetCell(hit.point), mouseAreaOrthographicSize);
            }
        }
    }

    public void focusOnArea(HexCell hexCell, float orthographicSize)
    {
        Area area = hexCell.transform.GetComponentInChildren<Area>();
        Debug.Log("Area: RegionId: " + area.region.GetRegionId());

        if(currentRegion.GetRegionId() != area.region.GetRegionId())
            return;

        Debug.Log("In focus on area");
        currentHexCell = hexCell;
        currentArea = area;
        Vector3 focusPosition = hexCell.transform.position;

        SetAreaFocus(true);

        currentSize = orthographicSize;
        currentPosition = new Vector3(focusPosition.x, focusPosition.y + 12.8f, focusPosition.z - 14.5f);
        currentRotation = orthographicAreaRotation;
    }

    public void focusOnRegion(HexCell hexCell, float orthographicSize)
    {
        Area area = hexCell.GetComponentInChildren<Area>();
        Region region = area.region;
        Debug.Log("Region: RegionId: " + region.GetRegionId());
        if(region.GetRegionId() == -1)
            return;

        Debug.Log("In focus on region");

        currentRegion = region;
        float maxX,minX, maxZ, minZ;
        positions = new Vector3[4];
        Vector3 leftPos, rightPos, topPos, bottomPos;
        positions[0] = leftPos = region.left.transform.position;
        positions[1] = rightPos = region.right.transform.position;
        positions[2] = topPos = region.top.transform.position;
        positions[3] = bottomPos = region.bottom.transform.position;

        float[] xArr = new float[4];
        float[] zArr = new float[4];

        xArr[0] = leftPos.x;
        xArr[1] = rightPos.x;
        xArr[2] = topPos.x;
        xArr[3] = bottomPos.x;
        zArr[0] = leftPos.z;
        zArr[1] = rightPos.z;
        zArr[2] = topPos.z;
        zArr[3] = bottomPos.z;

        maxX = Mathf.Max(xArr);
        minX = Mathf.Min(xArr);
        maxZ = Mathf.Max(zArr);
        minZ = Mathf.Min(zArr);

        Vector3 focusPosition = new Vector3((maxX + minX) / 2, 150f, (maxZ + minZ) / 2);
        currentSize = GetRequiredOrthographicSize(mainCamera, positions);
        currentPosition = focusPosition;
        currentRotation = orthographicRegionRotation;


        regionSize = currentSize;
        regionPosition = currentPosition;
        regionRotation = currentRotation;

        SetRegionFocus(true);
    }

    public float GetRequiredOrthographicSize(Camera orthographicCamera, params Vector3[] positionBounds)
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
