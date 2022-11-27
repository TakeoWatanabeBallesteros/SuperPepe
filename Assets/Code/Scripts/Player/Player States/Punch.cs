using UnityEngine;
using FSM;

public class Punch : StateBase
{
    private PlayerFSM _fsm;

    public Punch(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.animator.SetTrigger(_fsm.animIDPunch);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        base.OnLogic();
    }

    public override void OnExit()
    {
        _fsm.punchCombo = 0;
        _fsm.animator.SetInteger(_fsm.animIDPunchCombo, _fsm.punchCombo);
        base.OnExit();
    }
}