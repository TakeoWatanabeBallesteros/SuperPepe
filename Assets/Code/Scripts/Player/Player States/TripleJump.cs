using UnityEngine;
using FSM;

public class TripleJump : StateBase
{
    private PlayerFSM _fsm;

    public TripleJump(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
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