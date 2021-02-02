using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Add ScriptableObjects/Action - BlankGlobalAction")]
public class GlobalAction : Action
{
    [Header("可用前需完成的措施(全局)")]
    public List<GlobalAction> preFinishActions;
    private bool _isEnabled;
    public override void finishAction(Area area)
    {
        actionEffect();
        Stage.AddActionFinishCount(this);
    }
    public virtual void actionEffect() { }
    public bool enabled()
    {
        return preFinishActions.Find(action => !action.finishedOnce()) == null
                && preFinishInfos.Find(info => !info.IsFinished()) == null;
    }
}
