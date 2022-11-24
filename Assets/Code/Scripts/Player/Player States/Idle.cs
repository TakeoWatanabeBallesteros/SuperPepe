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
        _fsm.rb.velocity = Vector3.zero;
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm.animator.SetFloat(_fsm.animIDSpeed, 0);
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
