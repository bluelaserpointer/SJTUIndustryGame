using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Action : MonoBehaviour
{
    // Start is called before the first frame update
    public string actionName;
    [TextArea]
    public string description;
    public int timeCost, moneyCost;

    private Area area;
    private int progressedTime = -1;
    public void finishAction()
    {

    }
    public void isFinished()
    {

    }
    public void startAction()
    {
        progressedTime = 0;
    }
    public void cancelAction()
    {
        progressedTime = -1;
    }
    public bool isProgressing()
    {
        return progressedTime != -1 && progressedTime < timeCost;
    }
    public Area getArea()
    {
        return area;
    }
    public int getProgressedTime()
    {
        return progressedTime;
    }

    void Update()
    {
        if(isProgressing())
        {
            ++progressedTime;
            if(progressedTime >= timeCost)
            {
                finishAction();
            }
        }
    }
}
