using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Jump : StateBase
{
    private PlayerFSM _fsm;
    
    public Jump(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm._verticalVelocity = Mathf.Sqrt(_fsm.jumpHeight * -2f * _fsm.gravity);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm._verticalVelocity += _fsm.gravity * Time.deltaTime;
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
