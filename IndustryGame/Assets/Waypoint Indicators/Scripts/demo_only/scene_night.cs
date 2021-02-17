using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class scene_night : MonoBehaviour
{
    //This script manages:
    //Spawning mechanic

    //Root Canvas Vars
    /*
    public Canvas mainCanvas;
    public GameObject titleScreen;
    private bool instructionsWindowOpen;
    private GameObject instructionsWindowGameObject;
    private RectTransform instructionsWindowRect;
    public static bool canShoot;
    public static int spawnCount = 0;
    public static int spawnCountMax = 500;
    */



    //Scene Vars
    public float spawnSpeed = 5f;
    public Canvas levelCanvas;
    public TextMeshProUGUI spawnCountTextField;
    public TextMeshProUGUI spawnDescTextField;
    public TextMeshProUGUI wpDesc;
    private Transform spawnPos;
    private int exampleNum = 1;
    private int totalExamples = 4;
    public GameObject shapeSelectGameObject;
    private RectTransform shapeSelectRect;
    private Image shapeSelectImg;
    public Sprite slot01;
    public Sprite slot02;
    public Sprite slot03;
    public Sprite slot04;
    private GameObject newWaypoint;
    private Rigidbody newWaypointRB;
    private string shapeName;
    private GameObject btn_Day;
    private GameObject btn_Day_Selected;
    private GameObject btn_Night;
    private GameObject btn_Night_Selected;

    public Material skybox;


    void OnEnable()
    {
        event_manager.onOpenOptionScreen += HideUI;
        event_manager.onCloseOptionScreen += ShowUI;
    }

    void OnDisable()
    {
        event_manager.onOpenOptionScreen -= HideUI;
        event_manager.onCloseOptionScreen -= ShowUI;
    }

    void HideUI()
    {
        levelCanvas.enabled = false;
    }

    void ShowUI()
    {
        levelCanvas.enabled = true;
    }


    void Start()
    {
        //If this scene is active when play is hit, kill it and load the title screen
        if (scene_manager.curLevel == 0)
        {
            SceneManager.LoadScene("_root", LoadSceneMode.Single);
        }
        else
        {
            event_manager.StartLevel(); //Tell eveyone the level has started

            //Set skybox
            RenderSettings.skybox = skybox;


            //Hide/Show UI depending if Instruction Window is active
            if (scene_manager.instructionsWindowOpen)
            {
                HideUI(); //Do this if Instructions Window is up
            }
            else
            {
                ShowUI(); //Do this if Instructions Window closes 
            }

            spawnCountTextField.color = new Color32(153, 255, 0, 255);
            spawnDescTextField.color = new Color32(153, 255, 0, 255);
            wpDesc.color = new Color32(153, 255, 0, 255);

            //mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            //The player lives on the root level, but we need to target the spawnPos in it for Game Object shooting
            spawnPos = GameObject.Find("Player").transform.GetChild(0).transform;

            shapeSelectRect = shapeSelectGameObject.GetComponent<RectTransform>();
            shapeSelectImg = shapeSelectRect.GetComponent<Image>();
            //Cursor.lockState = CursorLockMode.Locked;

            spawnCountTextField.text = "0";
            spawnDescTextField.text = "Waypoints";

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Scene_" + scene_manager.curLevel));
        }
    }

    void Update()
    {
        //Cycle through Game Object types
        if (Input.GetKeyDown("q") || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            exampleNum--;
            if (exampleNum == 0)
            {
                exampleNum = totalExamples;
            }
        }

        if (Input.GetKeyDown("e") || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            exampleNum++;
            if (exampleNum == totalExamples + 1)
            {
                exampleNum = 1;
            }
        }

        if (scene_manager.curLevel != 0)
        {
            switch (exampleNum)
            {
                case 1:
                    shapeSelectImg.sprite = slot01;
                    shapeName = "1 Line Pointer";
                    wpDesc.text = "Line Pointer";
                    break;
                case 2:
                    shapeSelectImg.sprite = slot02;
                    shapeName = "2 Centered";
                    wpDesc.text = "Centered";
                    break;
                case 3:
                    shapeSelectImg.sprite = slot03;
                    shapeName = "3 Animated Ring";
                    wpDesc.text = "Animated Ring";
                    break;
                case 4:
                    shapeSelectImg.sprite = slot04;
                    shapeName = "4 Station";
                    wpDesc.text = "Destinaion Beacon";
                    break;
            }
        }


        //SHOOT
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) //Shoot primary weapon
        {
            if (scene_manager.canShoot)
            {
                if (scene_manager.spawnCount < scene_manager.spawnCountMax)
                {
                    Shoot(shapeName);
                    scene_manager.spawnCount++;
                }
            }

        }

        //UPDATE TEXT
        if (scene_manager.spawnCount < scene_manager.spawnCountMax)
        {
            spawnCountTextField.text = scene_manager.spawnCount.ToString();
            spawnDescTextField.text = "Waypoints";
        }
        else
        {
            spawnCountTextField.text = "MAX";
            spawnDescTextField.text = "Press [R] to reset";
        }

    }



    void Shoot(string str)
    {
        newWaypoint = Instantiate(Resources.Load("Night/" + str, typeof(GameObject)), spawnPos.position, Quaternion.identity) as GameObject;
        newWaypoint.name = str;
        newWaypointRB = newWaypoint.AddComponent<Rigidbody>();
        newWaypointRB = newWaypoint.GetComponent<Rigidbody>();
        newWaypointRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        newWaypointRB.AddForce(spawnPos.forward * spawnSpeed, ForceMode.Impulse); //give velocity forward
        Destroy(newWaypointRB.GetComponent<Rigidbody>(), 1f);
    }


}
