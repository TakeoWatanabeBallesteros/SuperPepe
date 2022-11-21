using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Walk : StateBase
{
    private PlayerFSM playerFsm;
    
    public Walk(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this.playerFsm = fsm;
    }

    public override void OnEnter()
    {
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
