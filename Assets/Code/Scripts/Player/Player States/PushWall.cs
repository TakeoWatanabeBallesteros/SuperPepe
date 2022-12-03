using UnityEngine;
using FSM;

public class PushWall : StateBase
{
    private PlayerFSM _fsm;
    private Transform playerPos;

    public PushWall(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _fsm.animator.SetBool(_fsm.animIDPushWall, true);

        _fsm.characterController.enabled = false;
        
        _fsm.transform.parent = _fsm.pushWallObj.transform;
        _fsm.transform.forward = _fsm.pushFwd;
        var forward = _fsm.transform.forward;
        _fsm.transform.localPosition = forward.normalized * -0.95f;
        _fsm.pushWallObj.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public override void OnLogic()
    {
        _fsm.pushWallObj.velocity = _fsm.transform.forward.normalized * (Time.deltaTime * 150);
        if (_fsm.moveInput.y <= 0) _fsm.pushWall = false;
        base.OnLogic();
    }

    public override void OnExit()
    {
        _fsm.pushWallObj.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        var pos = _fsm.transform.position;
        _fsm.transform.parent = null;
        // _fsm.transform.position = _fsm.pushWallObj.position + _fsm.transform.forward.normalized * -0.55f;
        _fsm.transform.position = pos;
        _fsm.pushWallObj = null;
        _fsm.animator.SetBool(_fsm.animIDPushWall, false);
        
        _fsm.characterController.enabled = true;
        base.OnExit();
    }
    
    
}