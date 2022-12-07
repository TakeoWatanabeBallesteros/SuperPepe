using UnityEngine;
using FSM;

public class Hit : StateBase
{
    private PlayerFSM _fsm;
    private float timer;

    public Hit(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        timer = 0;
        _fsm.animator.SetTrigger(_fsm.animIDHit);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        if (timer < 0.45f) timer += Time.deltaTime;
        else fsm.RequestStateChange("Idle");
        base.OnLogic();
        _fsm.characterController.Move(_fsm.hitDirection * 2 * Time.deltaTime +
                                      new Vector3(0, _fsm._verticalVelocity, 0) * Time.deltaTime);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}