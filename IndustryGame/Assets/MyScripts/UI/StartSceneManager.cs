using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public List<GameObject> decorationPrefabs;
    private int decorationPrefabIndex = 0;

    void Start()
    {
        decorationPrefabIndex = Random.Range(0, decorationPrefabs.Count);
        decorationPrefabs[decorationPrefabIndex].SetActive(true);
    }

    public void EnterGame ()
    {
        Debug.Log("Loading scene: UI");
        SceneManager.LoadScene("UI");
    }

    public void QuitGame ()
    {
        Application.Quit();
    }
}
