using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Jump03 : StateBase
{
    private PlayerFSM _fsm;
    
    public Jump03(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.jumpCombo = 0;
        _fsm.Jump(_fsm.jump03Height);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}