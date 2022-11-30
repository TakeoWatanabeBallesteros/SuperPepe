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
        _fsm.ApplyGravity();
        if (comboTime >= 0f)
        {
            comboTime -= Time.deltaTime;
            return;
        }
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
