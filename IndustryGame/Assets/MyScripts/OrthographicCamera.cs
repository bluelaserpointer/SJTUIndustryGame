using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class OrthographicCamera : MonoBehaviour
{
    private static OrthographicCamera instance;
    private Camera mainCamera;

    [Header("Orthographic Sizes")]
    public float worldOrthoSize;
    public float mouseAreaOrthoSize = 7f;
    public float keyAreaOrthoSize = 9f;
    public float maskAreaOrthoSize = 15f;    
    public float minObserveRegionSize = 30f;
    private float observeRegionSizeOffset = 5f;
    private float actualScrollSize;
    private float maxObserveRegionSize;
    private float currentSize;


    [Header("Speed for different lerps")]
    public float sizeSpeed = 7.5f;
    public float positionSpeed = 0.3f;
    public float regionObserveTranSpeed = 4.0f;
    public float regionObserveSizeSpeed = 10f;
    public float areaToRegionSpeedChangeOffset = 10f;
    public float areaToRegionSpeedChangeStep = 0.008f;
    private bool areaToRegionFlag = false;
    private float currentPositionSpeed;


    [Header("Transforms")]
    private Transform worldTransform;


    [Header("Positions")]
    public Vector3 worldPosition;
    public Vector3 areaPositionOffset;
    private Vector3 currentPosition;


    [Header("Rotations")]
    public Quaternion worldRotation;
    public float rotationSpeed = 0.1f;
    public float orthoRegionRotationX;
    public float orthoAreaRotationX;
    private Quaternion currentRotation;
    private Quaternion orthoRegionRotation;
    private Quaternion orthoAreaRotation;

    [Header("Focus Flags")]
    private bool regionFocus = false;
    private bool areaFocus = false;

    [Header("Current Components")]
    private HexGrid hexGrid;
    private HexCell currentHexCell;
    private Area currentArea;
    private int focusMask;

    [Header("Click Steps")]
    public float doubleClickStep = 0.5f;
    private float lastClickTime;

    [Header("AreaDetails HUD")]
    public GameObject AreaDetailsHUDGameObject;
    private AreaDetailsHUD AreaDetailsHUD;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
        Region region = Stage.GetRegions().Find(eachRegion => eachRegion.GetRegionId() == -1);
        mainCamera.transform.position = region.GetCenter();
        mainCamera.orthographicSize = worldOrthoSize;

        worldTransform = transform;

        actualScrollSize = mainCamera.orthographicSize;
        currentSize = mainCamera.orthographicSize;
        currentPosition = worldPosition = worldTransform.position;
        currentRotation = worldRotation = worldTransform.rotation;

        orthoAreaRotation = Quaternion.Euler(orthoAreaRotationX, orthoRegionRotation.y, orthoRegionRotation.z);
        orthoRegionRotation = Quaternion.Euler(orthoRegionRotationX, orthoRegionRotation.y, orthoRegionRotation.z);

        focusMask = LayerMask.GetMask("FocusMask");

        AreaDetailsHUD = AreaDetailsHUDGameObject.GetComponent<AreaDetailsHUD>();

        List<Region> regions = Stage.GetRegions(); 
        float maxSize = 0f;
        for(int i = 0; i < regions.Count; i++)
        {
            float currSize = GetRequiredOrthoSize(mainCamera, regions[i]);
            if(maxSize < currSize)
                maxSize = currSize;
        }

        maxObserveRegionSize = maxSize;
    }

    void Update () {
        HandleCameraFocus();
        AreaDetailsHUDGameObject.SetActive(currentArea != null);
        AreaDetailsHUD.SetMousePointingArea();
    }

    private void FixedUpdate() {
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, currentSize, Time.deltaTime * sizeSpeed);


        if(areaToRegionFlag)
        {
            if(Quaternion.Angle(transform.rotation, currentRotation) < areaToRegionSpeedChangeOffset)
            {
                currentPositionSpeed = Mathf.Lerp(currentPositionSpeed , positionSpeed, areaToRegionSpeedChangeStep);
                transform.position = Vector3.Lerp(transform.position, currentPosition, currentPositionSpeed);
            }else{
                currentPositionSpeed = Time.deltaTime * positionSpeed;
                transform.position = Vector3.Lerp(transform.position, currentPosition, Time.deltaTime * positionSpeed);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currentPosition, positionSpeed);
        }

        Quaternion currentAngle = Quaternion.Slerp(transform.rotation, currentRotation, rotationSpeed);
        transform.rotation = currentAngle;
    }

    /// <summary>
    /// 设置聚焦地区音频
    /// </summary>
    private void HandleFocusAreaAudio()
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

    /// <summary>
    /// 设置全局视角音频
    /// </summary>
    private void HandleGlobalAudio()
    {
        AreaWeatherSFXRandomPlayer.Silence();
        AreaBGMRandomPlayer.SetGlobalBgmList();
    }



    public void SetHexGrid(HexGrid hexGrid)
    {
        this.hexGrid = hexGrid;
    }

    /// <summary>
    /// 判断鼠标是否悬浮于UI元素
    /// </summary>
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    /// <summary>
    /// 设置Camera属性
    /// </summary>
    private void SetCurrentCameraParam(float currSize, Vector3 currPos, Quaternion currRot, bool areaToRegion)
    {
        currentSize = currSize;
        currentPosition = currPos;
        currentRotation = currRot;
        areaToRegionFlag = areaToRegion;
    }

    /// <summary>
    /// 设置Camera聚焦时屏蔽Object
    /// </summary>
    private void HandleCameraFocus() {
        if(mainCamera.orthographicSize <= maskAreaOrthoSize)
        {
            mainCamera.cullingMask &= ~(1 << 8); // 关闭层x
        }
        else
        {
            mainCamera.cullingMask |= (1 << 8);
        }
        
        if(!areaFocus && !regionFocus)
        {
            currentSize = actualScrollSize;
            
        }

        HandleRegionFocus();
    }

    /// <summary>
    /// 获取地区聚焦位置
    /// </summary>
    public Vector3 GetAreaFocusPosition(Vector3 focusPosition)
    {
        return new Vector3(focusPosition.x + areaPositionOffset.x, focusPosition.y + areaPositionOffset.y, focusPosition.z + areaPositionOffset.z);
    }

    /// <summary>
    /// 设置地区聚焦
    /// </summary>
    private void SetAreaFocus(bool value)
    {
        if(!areaFocus && !value)
            return;
        
        if(value)
            HandleFocusAreaAudio();
        else
            HandleGlobalAudio();

        areaFocus = value;
    }

    /// <summary>
    /// 执行地区聚焦
    /// </summary>
    public void FocusOnArea(HexCell hexCell, float OrthoSize)
    {
        Area area = hexCell.transform.GetComponentInChildren<Area>();

        currentHexCell = hexCell;
        currentArea = area;

        Vector3 focusPosition = hexCell.transform.position;

        SetCurrentCameraParam(OrthoSize, GetAreaFocusPosition(focusPosition), orthoAreaRotation, false);

        SetAreaFocus(true);
    }

    /// <summary>
    /// 管理地区聚焦
    /// </summary>
    private void HandleAreaFocus()
    {
        if (areaFocus)
        {
            // Moving with WASD
            HexCell focusHexCell = null;
            if (Input.GetKeyDown(KeyCode.W))
            {
                focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.NW);
                if (focusHexCell == null)
                    focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.NE);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.SE);
                if (focusHexCell == null)
                    focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.SW);
            }
            if (Input.GetKeyDown(KeyCode.A))
                focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.W);
            if (Input.GetKeyDown(KeyCode.D))
                focusHexCell = currentHexCell.GetNeighbor((HexDirection)(int)HexDirection.E);

            if (focusHexCell != null)
            {
                FocusOnArea(focusHexCell, keyAreaOrthoSize);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                actualScrollSize = worldOrthoSize;
                SetCurrentCameraParam(worldOrthoSize, worldPosition, worldRotation, false);
                currentArea = null;
                regionFocus = false;
                SetAreaFocus(false);
            }
        } else if (!IsPointerOverUIObject())
        {
            // Focusing with mouse
            Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                if(Input.GetMouseButtonDown(0))
                {
                    float timeSinceLastClick = Time.time - lastClickTime;
                    lastClickTime = Time.time;

                    if(timeSinceLastClick < doubleClickStep)
                    {
                        actualScrollSize = mouseAreaOrthoSize;
                        FocusOnArea(currentArea.GetComponentInParent<HexCell>(), mouseAreaOrthoSize);

                        regionFocus = false;
                        SetAreaFocus(true);
                    }else{
                        SetCurrentAreaByRaycast(hit);
                    }
                }
            }
        }
    }

    private void SetCurrentAreaByRaycast(RaycastHit hit)
    {
        HexCell hexCell = hexGrid.GetCell(hit.point);

        Area area = hexCell.transform.GetComponentInChildren<Area>();

        if(area.region.GetRegionId() == -1)
            return;

        currentHexCell = hexCell;
        currentArea = area;
        Debug.Log("Set current area by raycast");
    }

    /// <summary>
    /// 获取聚焦洲所需OrthoSize
    /// </summary>
    private float GetRequiredOrthoSize(Camera orthographicCamera, Region region)
    {
        // Stored width and height are just your Screen.Width and Screen.Height
        float aspectRatio = (float)16 / (float)9;
 
        // 1: If you have an orthographic size of 5, the viewport will contain exactly 10          
        // units of world space
        float yUnits = orthographicCamera.orthographicSize * 2.0f;
 
        // 2: horizontal size is based on units * aspect ratio
        float xUnits = yUnits * aspectRatio;

        // 3: calculate the viewport size based on these outer values
        float viewportSize = region.GetSizeInCamera(orthographicCamera);

        // 4: Convert to actual X width. This can now be considered as Viewport normalised        
        // to 1 length
        float newXUnits = viewportSize * xUnits;
 
        // 5: Get the Y Units which match these X Units                                            
        // -> Reverse xUnits = yUnits * aspectratio
        float newYUnits = newXUnits / aspectRatio;
 
        // 6: Divide by 2 to get the actual ortho size -> again reverse of getting YUnits          
        // using ortho size
        float newOrtho = newYUnits / 2.0f;
 
        // 7: Apply
        return newOrtho;
    }

    /// <summary>
    /// 执行洲聚焦
    /// </summary>
    private void FocusOnRegion()
    {   
        SetCurrentCameraParam(actualScrollSize , currentPosition, orthoRegionRotation, false);
    }

    /// <summary>
    /// 管理洲聚焦
    /// </summary>
    private void HandleRegionFocus()
    {
        // Handling mouse scroll
        HandleGlobalFocusScrollControl();        

        HandleAreaFocus();

        if(areaFocus)
            return;


        Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool raycasted = Physics.Raycast(inputRay, out hit);

        // Handling RegionFocus Control
        if(regionFocus == true)
            HandleRegionFocusControl();
        

        
        // // Handling Mouse Click Region
        // if(!IsPointerOverUIObject())
        // {
        //     if(raycasted)
        //     {
        //         // if(Input.GetMouseButtonDown(0))
        //         //     SetCurrentRegionByRaycast(hit);
        //         // FocusOnRegion(hexGrid.GetCell(hit.point));
        //     }
        // }
    }

    /// <summary>
    /// 设置摄像机全局的键鼠操作
    /// </summary>
    private void HandleGlobalFocusScrollControl()
    {
        if(!IsPointerOverUIObject())
        {
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                if(actualScrollSize > mouseAreaOrthoSize)
                    actualScrollSize -= regionObserveSizeSpeed;
            
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                if(actualScrollSize < worldOrthoSize)
                    actualScrollSize += regionObserveSizeSpeed;

            if(actualScrollSize < minObserveRegionSize)
            {
                if(currentArea != null)
                {
                    FocusOnArea(currentArea.GetComponentInParent<HexCell>(), mouseAreaOrthoSize);

                    regionFocus = false;
                    SetAreaFocus(true);
                }

            }else if(actualScrollSize >= minObserveRegionSize && actualScrollSize < maxObserveRegionSize)
            {
                FocusOnRegion();

                regionFocus = true;
                SetAreaFocus(false);

            }else if(actualScrollSize >= maxObserveRegionSize)
            {
                // Switch to world
                SetCurrentCameraParam(worldOrthoSize, worldPosition, worldRotation, false);

                regionFocus = false;
                SetAreaFocus(false);
            }
        }
    }


    /// <summary>
    /// 设置洲聚焦时的键鼠操作
    /// </summary>
    private void HandleRegionFocusControl()
    {

        if(Input.GetKey(KeyCode.S))
            currentPosition.z -= regionObserveTranSpeed;
        if(Input.GetKey(KeyCode.W))
            currentPosition.z += regionObserveTranSpeed;

        if(Input.GetKey(KeyCode.A))
            currentPosition.x -= regionObserveTranSpeed;
        if(Input.GetKey(KeyCode.D))
            currentPosition.x += regionObserveTranSpeed;

        // Exiting with esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            actualScrollSize = worldOrthoSize;
            SetCurrentCameraParam(worldOrthoSize, worldPosition, worldRotation, false);
            currentArea = null;
            SetAreaFocus(false);
            regionFocus = false;
        }
    }

    /// <summary>
    /// 获取鼠标悬浮地区
    /// </summary>
    public static Area GetMousePointingArea()
    {
        return instance != null ? instance.currentArea : null;
    }
}
