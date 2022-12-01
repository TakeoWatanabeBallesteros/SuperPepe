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
        _fsm.characterController.height = 0.75f;
        _fsm.characterController.center = new Vector3(0, 0.355f, 0);
    }

    public override void OnLogic()
    {
        _fsm.Move();
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
        _fsm.animator.SetBool(_fsm.animIDCrouch, _fsm.crouch);
        _fsm.characterController.height = 1.43f;
        _fsm.characterController.center = new Vector3(0, 0.71f, 0);
    }
}