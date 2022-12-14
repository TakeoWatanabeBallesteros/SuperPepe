using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Land : StateBase
{
    private PlayerFSM _fsm;
    
    public Land(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.animator.SetTrigger(_fsm.animIDLand);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm.ApplyGravity();
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
