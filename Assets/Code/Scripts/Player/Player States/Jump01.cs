using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Jump01 : StateBase
{
    private PlayerFSM _fsm;
    
    public Jump01(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.Jump(_fsm.jump01Height);
        // update animator if using character
        _fsm.animator.SetTrigger(_fsm.animIDJump);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm.Move();
        base.OnLogic();
        if (!(_fsm.rb.velocity.y < 0)) return;
        fsm.RequestStateChange("Fall");
    }

    public override void OnExit()
    {
        _fsm.jumpCombo += _fsm.jumpCombo == 2 ? -2 : 1;
        _fsm.animator.SetInteger(_fsm.animIDJumpCombo, _fsm.jumpCombo);
        base.OnExit();
    }
}
