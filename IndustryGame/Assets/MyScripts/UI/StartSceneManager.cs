using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    public Canvas startSceneHUD;
    public GameObject loadSceneWrapper;
    public List<Image> canvasBackgroundImages;
    public List<Text> canvasButtonTexts;
    public List<GameObject> decorationPrefabs;
    public List<Color> decorationColors;
    private int decorationPrefabIndex = 0;
    private Color decorationColor;

    void Start()
    {
        decorationPrefabIndex = Random.Range(0, decorationPrefabs.Count);
        decorationPrefabs[decorationPrefabIndex].SetActive(true);
        decorationColor = decorationColors[decorationPrefabIndex];

        foreach (var image in canvasBackgroundImages)
            image.color = decorationColor;

        foreach (var text in canvasButtonTexts)
            text.color = decorationColor;
    }

    public void EnterGame ()
    {
        loadSceneWrapper.SetActive(true);
        startSceneHUD.enabled = false;

        Debug.Log("Loading scene: UI");
        Invoke("LoadScene", 2f);
    }

    public void QuitGame ()
    {
        Application.Quit();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("UI");
    }
}
