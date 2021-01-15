using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EdgeFramework;

public class ProcedureManager 
{
    public ProcedureFSM FsmCtrl { get; private set; }


    public void OnInit(MonoBehaviour mono)
    {
        FsmCtrl = new ProcedureFSM(mono);
        FsmCtrl.AddState(new ProcedureLaunch(FsmCtrl));
        FsmCtrl.AddState(new ProcedureCheckUpdate(FsmCtrl));
        FsmCtrl.AddState(new ProcedureLoadingScene(FsmCtrl));
        FsmCtrl.AddState(new ProcedureMenu(FsmCtrl));
        

        FsmCtrl.AddState(new StateChooseClass(FsmCtrl));
        FsmCtrl.AddState(new StateClassContent(FsmCtrl));
        FsmCtrl.AddState(new StateGamePlay(FsmCtrl));
        FsmCtrl.AddState(new StateEndGame(FsmCtrl));
    }
    public void OnUpdate(float step)
    {
        FsmCtrl.OnUpdate(step);
    }
}
