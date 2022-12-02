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
        _fsm.animator.SetBool(_fsm.animIDPushWall, true);
    }

    public override void OnLogic()
    {
        // _fsm.Move();
        if (_fsm.moveInput.y <= 0) _fsm.pushWall = false;
        base.OnLogic();
    }

    public override void OnExit()
    {
        _fsm.animator.SetBool(_fsm.animIDPushWall, false);
        base.OnExit();
    }
    
    
}