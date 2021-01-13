using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    [TextArea]
    public List<string> infos = new List<string>();
    bool show = true;

    private int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentIndex)
        {
            case 0:
                // judge if user clicked XXX
                if (false)
                {
                    ++currentIndex;
                    show = true;
                }
                break;
            //TODO: judge currentIndex should increment and show should be true / false
        }
    }
    public int getCurrentIndex()
    {
        return currentIndex;
    }
    public void incCurrentIndex()
    {
        ++currentIndex;
    }
    public string currentInfo()
    {
        return currentIndex >= infos.Count ? null : infos[currentIndex];
    }

    public bool finished()
    {
        return currentInfo() == null;
    }
    public void finish()
    {
        currentIndex = infos.Count;
    }
}
