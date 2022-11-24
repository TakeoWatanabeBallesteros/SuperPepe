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
        Debug.Log("Jump");
        _fsm.jumpCombo++;
        _fsm.Jump(_fsm.jump01Height);
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
