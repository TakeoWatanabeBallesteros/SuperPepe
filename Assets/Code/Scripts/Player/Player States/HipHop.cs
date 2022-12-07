using UnityEngine;
using FSM;

public class HipHop : StateBase
{
    private PlayerFSM _fsm;
    private float timer;
    
    public HipHop(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.animator.SetTrigger(_fsm.animIDPunch);
        timer = 0;
        base.OnEnter();
    }

    public override void OnLogic()
    {
        if (timer < 1.0f) timer += Time.deltaTime;
        else fsm.RequestStateChange("Crouch");
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}