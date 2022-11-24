using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Jump02 : StateBase
{
    private PlayerFSM _fsm;
    
    public Jump02(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.jumpCombo++;
        _fsm.Jump(_fsm.jump02Height);
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