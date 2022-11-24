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
        if (_fsm.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            _fsm.jumpCombo = 0;
            fsm.RequestStateChange("Idle");
        }
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
