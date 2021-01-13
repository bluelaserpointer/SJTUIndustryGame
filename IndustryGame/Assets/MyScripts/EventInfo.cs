using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInfo : MonoBehaviour
{
    public string infoName;
    [TextArea]
    public string description;
    [TextArea]
    public string descriptionAfterFinish;
    public Dictionary<Ability, int> requiredAbilities = new Dictionary<Ability, int>();

    private bool _isFinished = false;
    public void finish()
    {
        _isFinished = true;
    }
    public bool isFinished()
    {
        return _isFinished;
    }
    public string getDescription()
    {
        return _isFinished ? descriptionAfterFinish : description;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
