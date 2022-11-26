using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Land : StateBase
{
    private PlayerFSM _fsm;
    private float comboTime = 0.2f;
    
    public Land(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        comboTime = 0.2f;
        _fsm.animator.SetTrigger(_fsm.animIDLand);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        if (comboTime >= 0f)
        {
            comboTime -= Time.deltaTime;
            return;
        }
        _fsm.jumpCombo = 0;
        _fsm.animator.SetInteger(_fsm.animIDJumpCombo, _fsm.jumpCombo);
        fsm.RequestStateChange(_fsm.moveInput != Vector2.zero ? "Walk" : "Idle");
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
