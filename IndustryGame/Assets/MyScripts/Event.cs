using System;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    public string eventName;
    [TextArea]
    public string description;

    [Serializable]
    public struct EventInfo
    {
        public string infoName;
        [TextArea]
        public string description;
        [TextArea]
        public string descriptionAfterFinish;
        [Header("解决条件")]
        public Condition successCondition;
        [Header("解决后能去除的环境效果")]
        public Buff buff;
        [Header("解决前提供的可用措施")]
        public List<Action> givenActionsBeforeSolve;
        [Header("解决后提供的可用措施")]
        public List<Action> givenActionsAfterSolve;

        private bool _isFinished;
        public void finish()
        {
            _isFinished = true;
            buff.removed();
        }
        public bool isFinished()
        {
            return _isFinished;
        }
        public string getDescription()
        {
            return _isFinished ? descriptionAfterFinish : description;
        }
    }
    public List<EventInfo> infos = new List<EventInfo>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(EventInfo info in infos) {
            if(!info.isFinished() && info.buff != null)
                info.buff.idle();
        }
    }
}
