using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Crouch : StateBase
{
    private PlayerFSM _fsm;

    public Crouch(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _fsm.animator.SetBool(_fsm.animIDCrouch, _fsm.crouch);
    }

    public override void OnLogic()
    {
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
        _fsm.animator.SetBool(_fsm.animIDCrouch, _fsm.crouch);
    }
}