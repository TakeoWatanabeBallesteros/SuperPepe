using UnityEngine;
using FSM;

public class Die : StateBase
{
    private PlayerFSM _fsm;

    public Die(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.animator.SetTrigger(_fsm.animIDDie);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm.ApplyGravity();
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}