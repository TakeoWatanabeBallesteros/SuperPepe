using UnityEngine;
using FSM;

public class Punch1 : StateBase
{
    private PlayerFSM _fsm;
    private float punchTime = 0.13f;

    public Punch1(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        punchTime = .13f;
        _fsm.animator.SetTrigger(_fsm.animIDPunch);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        base.OnLogic();
        if (punchTime >= 0) punchTime -= Time.deltaTime;
        else fsm.RequestStateChange(_fsm.punchCombo >= 2 ? "Punch02" : _fsm.moveInput != Vector2.zero ? "Walk" : "Idle");
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}