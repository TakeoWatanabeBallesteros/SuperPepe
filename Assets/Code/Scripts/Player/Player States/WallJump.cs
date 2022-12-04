using UnityEngine;
using FSM;

public class WallJump : StateBase
{
    private PlayerFSM _fsm;
    private Quaternion nextRot;

    public WallJump(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.animator.SetTrigger(_fsm.animIDJump);
        nextRot = Quaternion.LookRotation(-_fsm.transform.forward);
        _fsm.Jump(2);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        _fsm.transform.rotation = Quaternion.Lerp(_fsm.transform.rotation, nextRot, Time.deltaTime * 15);
        Vector3 direction = _fsm.transform.forward.normalized;
        _fsm.characterController.Move(new Vector3(direction.x * 3, _fsm._verticalVelocity, direction.z*3) * Time.deltaTime);
        base.OnLogic();
    }

    public override void OnExit()
    {
        _fsm.animator.ResetTrigger(_fsm.animIDJump);
        base.OnExit();
    }
}