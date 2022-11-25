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
        base.OnEnter();
    }

    public override void OnLogic()
    {
        if (_fsm.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle/Walk/Run"))
        {
            _fsm.jumpCombo = 0;
            _fsm.animator.SetInteger(_fsm.animIDJumpCombo, _fsm.jumpCombo);
            fsm.RequestStateChange(_fsm.moveInput != Vector2.zero ? "Walk" : "Idle");
            _fsm.animator.SetTrigger(_fsm.animIDLand);
        }
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
