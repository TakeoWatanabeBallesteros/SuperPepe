using UnityEngine;
using FSM;

public class PushWall : StateBase
{
    private PlayerFSM _fsm;

    public PushWall(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm.Move();
        _fsm.animator.SetBool(_fsm.animIDPushWall, _fsm.characterController.velocity.magnitude > 0);
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}