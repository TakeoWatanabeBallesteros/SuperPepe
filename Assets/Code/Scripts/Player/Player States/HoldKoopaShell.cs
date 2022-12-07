using UnityEngine;
using FSM;


public class HoldKoopaShell  : StateBase
{
    private PlayerFSM _fsm;

    public HoldKoopaShell(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        
    }

    public override void OnLogic()
    {
        base.OnLogic();
    }

    public override void OnExit()
    {
        
    }
}