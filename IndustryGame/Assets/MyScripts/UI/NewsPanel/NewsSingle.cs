using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NewsSingle : MonoBehaviour
{
    public News news;

    public Text NewsText;
    public Image NewsImage;
    public Button CloseButton;

    void Start()
    {
        CloseButton.onClick.AddListener(() => NewsPanel.instance.RemoveNews(news));
    }

    public void RefreshUI(News news)
    {
        this.news = news;
        NewsText.text = news.newsText;
        if (news.newsImage != null)
        {
            NewsImage.sprite = news.newsImage;
        }
    }
}
