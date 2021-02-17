using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class News
{
    public string newsText;
    public Sprite newsImage;

    public News(string newsText, Sprite newsImage)
    {
        this.newsText = newsText;
        this.newsImage = newsImage;
    }

    public News()
    { }
}
