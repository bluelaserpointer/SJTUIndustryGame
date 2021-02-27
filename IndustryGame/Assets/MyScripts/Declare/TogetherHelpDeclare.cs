using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogetherHelpDeclare : Declare
{
    public override string Description => "直到 " + targetMainEvent.name + " 得到解决， 它的环境问题危害-15%\n" +
        "当解决时事件完成评价高于A->名声+200 低于A->评价-5";

    public MainEvent targetMainEvent;

    public TogetherHelpDeclare() : base("共同保护宣言", Type.Support)
    {
        //TODO: Choose most damaged environment event
        targetMainEvent = null;
    }
    public override void DayIdle()
    {
        //TODO: edit
    }
}
