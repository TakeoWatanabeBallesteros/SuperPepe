using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using FSM;

public class BumDrop : StateBase
{
    private PlayerFSM _fsm;
    private bool canFall;

    public BumDrop(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        canFall = false;
        _fsm.animator.SetTrigger(_fsm.animIDBumDrop);
        _fsm.animator.ResetTrigger(_fsm.animIDFreeFall);
        _fsm.jumpCombo = 0;
        _fsm.animator.SetInteger(_fsm.animIDJumpCombo, _fsm.jumpCombo);
        _fsm.StartCoroutine(StartFall());
    }

    public override void OnLogic()
    {
        if(!canFall) return;
        base.OnLogic();
        _fsm.ApplyGravity();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private IEnumerator StartFall()
    {
        yield return new WaitForSeconds(.2f);
        canFall = true;
        _fsm._verticalVelocity = -15;
    }
}