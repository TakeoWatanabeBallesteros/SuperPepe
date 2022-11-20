using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Land : StateBase
{
    private PlayerFSM playerFsm;
    
    public Land(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this.playerFsm = fsm;
    }

    public override void OnEnter()
    {
        fsm.RequestStateChange("Idle");
        base.OnEnter();
    }

    public override void OnLogic()
    {
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
