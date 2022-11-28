using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class CrouchJump : StateBase
{
    private PlayerFSM _fsm;
    
    public CrouchJump(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.Jump(_fsm.jump03Height);
        // update animator if using character
        _fsm.animator.SetTrigger(_fsm.animIDJump);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        Vector3 direction = -_fsm.transform.forward.normalized;
        _fsm.characterController.Move(new Vector3(direction.x * 5, _fsm._verticalVelocity, direction.z*5) * Time.deltaTime);
        base.OnLogic();
        // if (!(_fsm.characterController.velocity.y < 0)) return;
        // fsm.RequestStateChange("Fall");
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}