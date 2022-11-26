using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Walk : StateBase
{
    private PlayerFSM _fsm;
    
    public Walk(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm.Move();
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
