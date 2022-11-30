using UnityEngine;
using FSM;

public class LongJump : StateBase
{
    private PlayerFSM _fsm;

    public LongJump(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        _fsm.Jump(_fsm.jump01Height);
        // update animator if using character
        _fsm.animator.SetTrigger(_fsm.animIDLongJump);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        Vector3 direction = _fsm.transform.forward.normalized;
        _fsm.characterController.Move(new Vector3(direction.x * 8, _fsm._verticalVelocity, direction.z*8) * Time.deltaTime);
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}