using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Idle : StateBase
{
    private PlayerFSM _fsm;
    private float idleTimer;
    
    public Idle(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        idleTimer = 0;
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm.ApplyGravity();
        _fsm.animator.SetFloat(_fsm.animIDSpeed, 0);
        base.OnLogic();
        if (idleTimer <= 10) idleTimer += Time.deltaTime;
        else if(idleTimer >= 10 && !_fsm.animator.GetBool(_fsm.animIDExtraIdle)) _fsm.animator.SetBool(_fsm.animIDExtraIdle, true);
    }

    public override void OnExit()
    {
        _fsm.animator.SetBool(_fsm.animIDExtraIdle, false);
        base.OnExit();
    }
}
