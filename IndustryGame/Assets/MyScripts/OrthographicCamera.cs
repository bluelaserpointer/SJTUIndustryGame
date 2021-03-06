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
    public float maskAreaOrthoSize = 20f;
    public float minObserveRegionSize = 30f;
    public float operatableSizeOffset = 10f;
    public float actualScrollSize;
    private float maxObserveRegionSize;
    private float currentSize;


    [Header("Speed for different lerps")]
    public float sizeSpeed = 7.5f;
    public float positionSpeed = 0.2f;
    public float regionObserveTranSpeed = 4.0f;
    public float regionObserveSizeSpeed = 10f;
    public float areaToRegionSpeedChangeOffset = 30f;
    public float areaToRegionSpeedChangeStep = 0.025f;
    private bool modeChangeFlag = false;
    private float currentPositionSpeed;


    [Header("Transforms")]
    private Transform worldTransform;


    [Header("Positions")]
    public Vector3 worldPosition;
    public Vector3 areaPositionOffset;
    public Vector3 areaEscapePositionOffset;
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
    private bool focusFlag = false;

    [Header("Current Components")]
    private HexCell currentHexCell;
    private Area currentArea;
    private Region currentRegion;
    private int focusMask;

    [Header("Click Steps")]
    public float doubleClickStep = 0.5f;
    private float lastClickTime = 0.0f;
    public float regionBorderChangeStep = 1f;
    public KeyCode lastDirection;
    public float lastRegionBorderChangeTime;
    public float totalRegionBorderChangeTime;

    [Header("AreaDetails HUD")]
    public GameObject AreaDetailsHUDGameObject;
    [Header("RegionDetails HUD")]
    public GameObject RegionDetailsHUDGameObject;
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
        for (int i = 0; i < regions.Count; i++)
        {
            float currSize = GetRequiredOrthoSize(mainCamera, regions[i]);
            regions[i].observeOrthoSize = currSize;

            if (maxSize < currSize)
                maxSize = currSize;
        }

        maxObserveRegionSize = maxSize;
    }

    void Update()
    {
        HandleCameraFocus();
        AreaDetailsHUDGameObject.SetActive(areaFocus);
        AreaDetailsHUD.SetMousePointingArea();
        RegionDetailsHUDGameObject.SetActive(regionFocus);
        RegionDetailsHUD.SetMousePointingRegion();
    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(mainCamera.orthographicSize - currentSize) < operatableSizeOffset)
            focusFlag = false;

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, currentSize, Time.deltaTime * sizeSpeed);


        if (modeChangeFlag)
        {
            if (Quaternion.Angle(transform.rotation, currentRotation) < areaToRegionSpeedChangeOffset)
            {
                currentPositionSpeed = Mathf.Lerp(currentPositionSpeed, positionSpeed, areaToRegionSpeedChangeStep);
                transform.position = Vector3.Lerp(transform.position, currentPosition, currentPositionSpeed);
            }
            else
            {
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

        if (animals.Count > 0)
        {
            AreaAnimalSFXRandomPlayer.setAnimalList(animals);
            int dangerTypeNum = EnumHelper.GetMaxEnum<SpeciesDangerType>();
            for (int i = dangerTypeNum; i > 0; i--)
            {
                if (animals[i].Count > 0)
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

    /// <summary>
    /// 设置全局视角音频
    /// </summary>
    private void HandleGlobalAudio()
    {
        AreaWeatherSFXRandomPlayer.Silence();
        AreaBGMRandomPlayer.SetGlobalBgmList();
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
    private void SetCurrentCameraParam(float currSize, Vector3 currPos, Quaternion currRot, bool modeChange)
    {
        if (modeChange)
            actualScrollSize = currSize;

        currentSize = currSize;
        currentPosition = currPos;
        currentRotation = currRot;
        modeChangeFlag = modeChange;
    }

    /// <summary>
    /// 设置Camera聚焦时屏蔽Object
    /// </summary>
    private void HandleCameraFocus()
    {
        if (mainCamera.orthographicSize <= maskAreaOrthoSize)
        {
            mainCamera.cullingMask &= ~(1 << 8); // 关闭层x
        }
        else
        {
            mainCamera.cullingMask |= (1 << 8);
        }

        // if (!areaFocus && !regionFocus)
        // {
            // currentSize = actualScrollSize;
        // }

        // HandleRegionFocus();

        HandleGlobalViewControl();
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
        if (!areaFocus && !value)
            return;

        // if(value)
        // HandleFocusAreaAudio();
        // else
        HandleGlobalAudio();

        areaFocus = value;
    }

    /// <summary>
    /// 执行地区聚焦
    /// </summary>
    public void FocusOnAreaByHexCell(HexCell hexCell, float OrthoSize, bool modeChange)
    {
        if (hexCell == null)
            return;

        Area area = hexCell.transform.GetComponentInChildren<Area>();
        
        if(area.region.GetRegionId() == -1)
            return;

        focusFlag = true;


        currentHexCell = hexCell;
        currentArea = area;

        Vector3 focusPosition = hexCell.transform.position;

        SetCurrentCameraParam(OrthoSize, GetAreaFocusPosition(focusPosition), orthoAreaRotation, modeChange);

        SetAreaFocus(true);
    }

    /// <summary>
    /// 执行地区聚焦
    /// </summary>
    public void FocusOnAreaByRaycast(RaycastHit hit, float OrthoSize)
    {
        HexCell hexCell = Stage.GetHexGrid().GetCell(hit.point);

        Area area = hexCell.transform.GetComponentInChildren<Area>();

        if (area.region.GetRegionId() == -1)
            return;

        currentHexCell = hexCell;
        currentArea = area;
        currentRegion = area.region;

        Vector3 focusPosition = hexCell.transform.position;

        SetCurrentCameraParam(OrthoSize, GetAreaFocusPosition(focusPosition), orthoAreaRotation, false);

        regionFocus = false;
        SetAreaFocus(true);
    }

    /// <summary>
    /// 管理地区聚焦
    /// </summary>
    private void HandleAreaFocus()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Vector3 prevPos = currentPosition;

            FocusOnRegion(currentRegion, true);

            float angle = Quaternion.Angle(currentRotation, orthoAreaRotation);
            float ratio = 0.5f;
            if(ratio < 1f)
            {
                currentRotation *= Quaternion.AngleAxis(angle * ratio, Vector3.left);
                
                float highestY = GetAreaFocusPosition(Stage.GetHighestPosition()).y;
                float dstY = currentPosition.y - (currentPosition.y - highestY) * ratio;

                currentPosition = prevPos;

                if(dstY < highestY)
                    currentPosition.y = highestY;
                else
                    currentPosition.y = dstY;

                currentSize -= 0.5f * (currentSize - mouseAreaOrthoSize);

                currentPosition.z += areaEscapePositionOffset.z;

                SetAreaFocus(false);
            }
        }
    }

    private void SetCurrentAreaByRaycast(RaycastHit hit)
    {
        HexCell hexCell = Stage.GetHexGrid().GetCell(hit.point);

        Area area = hexCell.transform.GetComponentInChildren<Area>();

        if (area.region.GetRegionId() == -1)
            return;

        currentHexCell = hexCell;
        currentArea = area;
        currentRegion = area.region;

        SetAreaFocus(true);

        // Debug.Log("Set current area by raycast");
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
    private void FocusOnRegion(Region region, bool modeChange)
    {   
        // Debug.Log("Focus on region");

        if(region == null || region.GetRegionId() == -1)
            return;

        focusFlag = true;

        regionFocus = true;
        currentRegion = region;
        SetCurrentCameraParam(region.observeOrthoSize , region.GetCenter(), orthoRegionRotation, modeChange);
    }

    private void ChangeFocusRegion(Region region, bool modeChange)
    {
        if(region == null || region.GetRegionId() == -1)
            return;

        focusFlag = true;

        regionFocus = true;
        SetAreaFocus(false);
        currentArea = null;
        currentRegion = region;
        SetCurrentCameraParam(region.observeOrthoSize , region.GetCenter(), orthoRegionRotation, modeChange);
    }

    /// <summary>
    /// 管理洲聚焦
    /// </summary>
    private void HandleRegionFocus()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCurrentCameraParam(worldOrthoSize, worldPosition, worldRotation, true);
        }

        if (focusFlag == false)
        {
            if (!IsPointerOverUIObject())
            {
                Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool raycasted = Physics.Raycast(inputRay, out hit);
                if (raycasted)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Double Click

                        float timeSinceLastClick = Time.time - lastClickTime;
                        lastClickTime = Time.time;
                        if (timeSinceLastClick < doubleClickStep)
                        {
                            FocusOnAreaByRaycast(hit, mouseAreaOrthoSize);
                        }else{
                            SetCurrentAreaByRaycast(hit);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 设置摄像机全局的键鼠操作
    /// </summary>
    private void HandleGlobalFocusScrollControl()
    {
        if (!IsPointerOverUIObject())
        {

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                if (actualScrollSize > mouseAreaOrthoSize)
                    actualScrollSize -= regionObserveSizeSpeed;

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                if (actualScrollSize < worldOrthoSize)
                    actualScrollSize += regionObserveSizeSpeed;

            if (actualScrollSize < minObserveRegionSize)
            {
                if (currentArea != null)
                {
                    FocusOnAreaByHexCell(currentArea.GetComponentInParent<HexCell>(), mouseAreaOrthoSize, true);
                }

            }
            else if (actualScrollSize >= minObserveRegionSize && actualScrollSize < maxObserveRegionSize)
            {
                FocusOnRegion(currentRegion, true);


            }
            else if (actualScrollSize >= maxObserveRegionSize)
            {
                // Switch to world
                SetCurrentCameraParam(worldOrthoSize, worldPosition, worldRotation, true);

                regionFocus = false;
                SetAreaFocus(false);
            }
        }
    }

    private void HandleGlobalViewControl()
    {
        // bool areaToRegionFlag = false;

        // HandleAreaFocus(ref areaToRegionFlag);

        // if (areaToRegionFlag)
        //     return;

        if(currentSize >= mouseAreaOrthoSize && currentSize < minObserveRegionSize){
            HandleAreaFocus();

        }else if(currentSize >= minObserveRegionSize){
            HandleRegionFocus();            
        }

        float speed = regionObserveTranSpeed * ((currentSize - mouseAreaOrthoSize) / worldOrthoSize);

        if(Input.GetKey(KeyCode.S))
        {
            currentPosition.z -= speed;
        }

        if(Input.GetKey(KeyCode.W))
        {
            currentPosition.z += speed;
        }

        if(Input.GetKey(KeyCode.A))
        {
            currentPosition.x -= speed;
        }

        if(Input.GetKey(KeyCode.D))
        {
            currentPosition.x += speed;
        }

        RaycastHit centerHit;
        Physics.Raycast(mainCamera.transform.position,mainCamera.transform.forward, out centerHit);
        HexCell centerHexCell = Stage.GetHexGrid().GetCell(centerHit.point);
        // centerHexCell.GetComponentInParent<Highlighter>().enabled = true;
        Vector3 centerPosition = GetAreaFocusPosition(centerHexCell.transform.position);

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentSize > mouseAreaOrthoSize)
            {
                float angle = Quaternion.Angle(currentRotation, orthoAreaRotation);
                float ratio = regionObserveSizeSpeed / (currentSize - mouseAreaOrthoSize);
                if(ratio < 1f)
                {
                    currentRotation *= Quaternion.AngleAxis(angle * ratio, Vector3.left);
                    
                    float centerY = centerPosition.y;
                    float dstY = currentPosition.y - (currentPosition.y - centerY) * ratio;

                    // If dstY is smaller than focusPositionY, reposition
                    if(dstY < centerY)
                    {
                        // SetCurrentCameraParam(mouseAreaOrthoSize, centerPosition, orthoAreaRotation, false);
                    }
                    else
                    {
                        currentPosition.y = dstY;

                        float zChange = Mathf.Abs((currentPosition.z - centerPosition.z) * ratio);
                        if(currentPosition.z < centerPosition.z)
                        {
                            currentPosition.z += zChange;
                        }else{
                            currentPosition.z -= zChange;
                        }
                        currentSize -= regionObserveSizeSpeed;
                    }

                }else
                {
                    // If ratio is larger than 1.0f, reposition

                    // currentSize = mouseAreaOrthoSize;
                    // SetCurrentCameraParam(mouseAreaOrthoSize, centerPosition, orthoAreaRotation, false);
                }


            }else{
                // If currentSize is accidentally smaller than mouseAreaOrthoSize, reposition

                // SetCurrentCameraParam(mouseAreaOrthoSize, centerPosition, orthoAreaRotation, false);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentSize < worldOrthoSize)
            {
                float angle = Quaternion.Angle(currentRotation, worldRotation);
                float ratio = regionObserveSizeSpeed / (worldOrthoSize - currentSize);
                if(ratio < 1f)
                {
                    currentRotation *= Quaternion.AngleAxis(angle * ratio, Vector3.right);
                    currentPosition.y += (worldPosition.y - currentPosition.y) * ratio;


                    currentPosition.z += (worldPosition.z - currentPosition.z) * ratio;
                    float zChange = Mathf.Abs((worldPosition.z - currentPosition.z) * ratio);
                    if(currentPosition.z > worldPosition.z)
                    {
                        currentPosition.z -= zChange;
                    }else{
                        currentPosition.z += zChange;
                    }

                    currentSize += regionObserveSizeSpeed;
                }
                // else
                    // currentSize = worldOrthoSize;
            }
        }
    }

    /// <summary>
    /// 设置洲聚焦时的键鼠操作
    /// </summary>
    private void HandleRegionFocusControl()
    {
        float x = currentPosition.x;
        float z = currentPosition.z;
        float xMinLimit = currentRegion.GetLeft();
        float xMaxLimit = currentRegion.GetRight();
        float zMinLimit = currentRegion.GetBottom() + areaPositionOffset.z;
        float zMaxLimit = currentRegion.GetTop() + areaPositionOffset.z;

        
        // float currTime = Time.time;

        // Vector3 prevPosition = currentPosition;

        // Exiting with esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCurrentCameraParam(worldOrthoSize, worldPosition, worldRotation, true);
        }
    }

    /// <summary>
    /// 获取鼠标悬浮地区
    /// </summary>
    public static Area GetMousePointingArea()
    {
        return instance != null ? instance.currentArea : null;
    }

    public static Region GetMousePointingRegion()
    {
        return instance != null ? instance.currentRegion : null;
    }
}
