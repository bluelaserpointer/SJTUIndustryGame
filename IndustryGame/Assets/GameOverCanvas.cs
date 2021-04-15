using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    public static GameOverCanvas instance;
    public Text DateScore;
    public Text AnimalText;
    public Image AnimalImage;
    public Button BackToMainMenuButton;
    public Button QuitButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        BackToMainMenuButton.onClick.AddListener(() => BackToMainMenu());
        QuitButton.onClick.AddListener(() => QuitGame());
        gameObject.SetActive(false);
    }


    void ShowScore(string dateScore, string animalName, Sprite animalSprite)
    {
        DateScore.text = dateScore + "天";
        AnimalText.text = animalName;
        AnimalImage.sprite = animalSprite;
        gameObject.SetActive(true);
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("GameStart");
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
