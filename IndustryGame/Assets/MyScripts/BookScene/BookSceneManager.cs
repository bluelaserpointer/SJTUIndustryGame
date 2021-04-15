using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BookSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip FlippingClip;
    public Sprite rightNext;
    void Start()
    {
        rightNext = GameObject.Find("RightNext").GetComponent<Image>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFlippingSound()
    {
        AudioSource.PlayClipAtPoint(FlippingClip, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        rightNext = GameObject.Find("RightNext").GetComponent<Image>().sprite;
        if (rightNext.name == "TransparentGraybackgtound")
        {
            Invoke("StartGame", 0.01f);
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("UI");
    }

}
