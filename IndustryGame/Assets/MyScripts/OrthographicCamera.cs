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

    [Header("Current Components")]
    private Area currentArea;
    private Region currentRegion;

    [Header("Click Steps")]
    public float doubleClickStep = 0.5f;
    private float lastClickTime = 0.0f;

    [Header("AreaDetails HUD")]
    public GameObject AreaDetailsHUDGameObject;

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
        AreaDetailsHUDGameObject.SetActive(false);
    }

    void Update()
    {
        //聚焦时的物体屏蔽
        if (mainCamera.orthographicSize <= maskAreaOrthoSize)
        {
            mainCamera.cullingMask &= ~(1 << 8); // 关闭层x
        }
        else
        {
            mainCamera.cullingMask |= (1 << 8);
        }
        //camera focus
        HandleClick();
        HandleMouseScroll();
        HandleEsc();
        //camera movement
        HandleMove();

        RegionDetailsHUD.SetMousePointingRegion();
    }

    private void FixedUpdate()
    {
        //摄像头的平滑移动
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

        transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, rotationSpeed);
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
    public static bool IsPointerOverUIObject()
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
    /// 获取地区聚焦位置
    /// </summary>
    public Vector3 GetAreaFocusPosition(Vector3 focusPosition)
    {
        return new Vector3(focusPosition.x + areaPositionOffset.x, focusPosition.y + areaPositionOffset.y, focusPosition.z + areaPositionOffset.z);
    }

    /// <summary>
    /// 执行地区聚焦
    /// </summary>
    public void FocusOnArea(Area area, float OrthoSize)
    {
        Vector3 focusPosition = area.transform.position;
        SetCurrentCameraParam(OrthoSize, GetAreaFocusPosition(focusPosition), orthoAreaRotation, false);
    }

    private void SetCurrentArea(Area area)
    {
        currentArea = area;
        currentRegion = area.region;
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
        if(region == null || region.GetRegionId() == -1)
            return;
        currentRegion = region;
        SetCurrentCameraParam(region.observeOrthoSize, region.GetCenter(), orthoRegionRotation, modeChange);
    }
    /// <summary>
    /// 管理鼠标滚轮操作
    /// </summary>
    private void HandleMouseScroll()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput == 0.0)
            return;
        RaycastHit centerHit;
        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out centerHit);
        HexCell centerHexCell = Stage.GetHexGrid().GetCell(centerHit.point);
        // centerHexCell.GetComponentInParent<Highlighter>().enabled = true;
        Vector3 centerPosition = GetAreaFocusPosition(centerHexCell.transform.position);

        if (scrollInput > 0)
        {
            if (currentSize > mouseAreaOrthoSize)
            {
                float angle = Quaternion.Angle(currentRotation, orthoAreaRotation);
                float ratio = regionObserveSizeSpeed / (currentSize - mouseAreaOrthoSize);
                if (ratio < 1f)
                {
                    currentRotation *= Quaternion.AngleAxis(angle * ratio, Vector3.left);

                    float centerY = centerPosition.y;
                    float dstY = currentPosition.y - (currentPosition.y - centerY) * ratio;

                    // If dstY is smaller than focusPositionY, reposition
                    if (dstY < centerY)
                    {
                        // SetCurrentCameraParam(mouseAreaOrthoSize, centerPosition, orthoAreaRotation, false);
                    }
                    else
                    {
                        currentPosition.y = dstY;

                        float zChange = Mathf.Abs((currentPosition.z - centerPosition.z) * ratio);
                        if (currentPosition.z < centerPosition.z)
                        {
                            currentPosition.z += zChange;
                        }
                        else
                        {
                            currentPosition.z -= zChange;
                        }
                        currentSize -= regionObserveSizeSpeed;
                    }

                }
                else
                {
                    // If ratio is larger than 1.0f, reposition

                    // currentSize = mouseAreaOrthoSize;
                    // SetCurrentCameraParam(mouseAreaOrthoSize, centerPosition, orthoAreaRotation, false);
                }


            }
            else
            {
                // If currentSize is accidentally smaller than mouseAreaOrthoSize, reposition

                // SetCurrentCameraParam(mouseAreaOrthoSize, centerPosition, orthoAreaRotation, false);
            }
        }
        else //scrollInput < 0
        {
            if (currentSize < worldOrthoSize)
            {
                float angle = Quaternion.Angle(currentRotation, worldRotation);
                float ratio = regionObserveSizeSpeed / (worldOrthoSize - currentSize);
                if (ratio < 1f)
                {
                    currentRotation *= Quaternion.AngleAxis(angle * ratio, Vector3.right);
                    currentPosition.y += (worldPosition.y - currentPosition.y) * ratio;


                    currentPosition.z += (worldPosition.z - currentPosition.z) * ratio;

                    float zChange = Mathf.Abs((worldPosition.z - currentPosition.z) * ratio);

                    if (currentPosition.z > worldPosition.z)
                    {
                        currentPosition.z -= zChange;
                    }
                    else
                    {
                        currentPosition.z += zChange;
                    }

                    currentSize += regionObserveSizeSpeed;

                    AreaDetailsHUDGameObject.SetActive(true);
                }
                // else
                // currentSize = worldOrthoSize;
            }
        }
    }
    /// <summary>
    /// 管理ESC键入操作
    /// </summary>
    private void HandleEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentSize >= minObserveRegionSize) //聚焦远时按ESC返回世界地图
            {
                SetCurrentCameraParam(worldOrthoSize, worldPosition, worldRotation, true);
            }
            else //聚焦近时按ESC稍微远离
            {
                Vector3 prevPos = currentPosition;

                FocusOnRegion(currentRegion, true);

                float angle = Quaternion.Angle(currentRotation, orthoAreaRotation);
                float dstRatio = currentSize - Mathf.Abs(currentSize - minObserveRegionSize) / Mathf.Abs(minObserveRegionSize - mouseAreaOrthoSize);

                float ratio = Mathf.Max(0.5f, dstRatio);

                if (ratio < 1f)
                {
                    currentRotation *= Quaternion.AngleAxis(angle * ratio, Vector3.left);

                    float highestY = GetAreaFocusPosition(Stage.GetHighestPosition()).y;
                    float dstY = currentPosition.y - (currentPosition.y - highestY) * ratio;

                    currentPosition = prevPos;

                    if (dstY < highestY)
                        currentPosition.y = highestY;
                    else
                        currentPosition.y = dstY;

                    currentSize -= ratio * (currentSize - mouseAreaOrthoSize);

                    currentPosition.z += areaEscapePositionOffset.z;
                }
            }
            AreaDetailsHUDGameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 管理点击
    /// </summary>
    private void HandleClick()
    {
        if (!IsPointerOverUIObject())
        {
            // detect any area click
            Ray inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                Area area = Stage.GetHexGrid().GetCell(hit.point).transform.GetComponentInChildren<Area>();
                if(!area.region.IsOcean && Input.GetMouseButtonDown(0)) // area click
                {
                    //show area inspector
                    AreaDetailsHUDGameObject.SetActive(true);
                    SetCurrentArea(area);
                    // double click invokes focus
                    if (Time.time - lastClickTime < doubleClickStep)
                    {
                        FocusOnArea(area, mouseAreaOrthoSize);
                    }
                    lastClickTime = Time.time;
                }
            }
        }
    }
    /// <summary>
    /// 管理移动键入
    /// </summary>
    private void HandleMove()
    {
        float cameraMoveSpeed = regionObserveTranSpeed * ((currentSize - mouseAreaOrthoSize) / worldOrthoSize);
        float moveInput;
        if ((moveInput = Input.GetAxis("Horizontal")) != 0.0)
        {
            currentPosition.x += cameraMoveSpeed * moveInput;
        }
        if ((moveInput = Input.GetAxis("Vertical")) != 0.0)
        {
            currentPosition.z += cameraMoveSpeed * moveInput;
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
