using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsPanel : MonoBehaviour
{
    public static NewsPanel instance;

    private List<News> NewsList = new List<News>();

    [Header("News的Prefab")]
    public GameObject NewsSingle;
    public GameObject NewsSingleGeneratePosition;

    private List<GameObject> GeneratedNews = new List<GameObject>();


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        News a = new News("Testing1", null);
        News b = new News("Testing2", null);
        NewsList.Add(a);
        NewsList.Add(b);

        RefreshNews();
    }

    public void AddNews(string newsDescription, Sprite newsImage)
    {
        News news = new News(newsDescription, newsImage);
        NewsList.Add(news);
        RefreshNews();
    }

    public void RefreshNews()
    {
        Helper.ClearList(GeneratedNews);

        foreach (News news in NewsList)
        {
            GameObject clone = Instantiate(NewsSingle, NewsSingleGeneratePosition.transform, false);
            clone.GetComponent<NewsSingle>().RefreshUI(news);
            GeneratedNews.Add(clone);
        }
    }

    public void RemoveNews(News news)
    {
        NewsList.Remove(news);
        RefreshNews();
    }

    void Update()
    {
        // RefreshNews();
    }


}
