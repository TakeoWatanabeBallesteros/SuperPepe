using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Idle : StateBase
{
    private PlayerFSM _fsm;
    
    public Idle(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
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