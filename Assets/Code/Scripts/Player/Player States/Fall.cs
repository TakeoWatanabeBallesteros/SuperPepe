using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Fall : StateBase
{
    private PlayerFSM _fsm;

    public Fall(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        // update animator if using character
        _fsm.animator.SetTrigger(_fsm.animIDFreeFall);
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
