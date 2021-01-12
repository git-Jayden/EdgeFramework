using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureSplash : ProcedureBase
{
    public ProcedureSplash(FSM _fsm)
  : base(_fsm, StateDefine.PROCEDURE_SPLASH)
    {

    }
    public override void OnEnter()
    {
        // TODO: 这里可以播放一个 Splash 动画
        GameObject.FindObjectOfType<Canvas>().transform.Find("WindRoot/Image").GetComponent<CameraFadeInOut>().fadeIn();
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate(float step)
    {
       
    }
}
