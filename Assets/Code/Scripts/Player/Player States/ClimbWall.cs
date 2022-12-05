using System.Collections;
using UnityEngine;
using FSM;

public class ClimbWall : StateBase
{
    private PlayerFSM _fsm;
    private float timer;

    public ClimbWall(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this._fsm = fsm;
    }

    public override void OnEnter()
    {
        timer = 0;
        _fsm.StartCoroutine(ClimbMovement());
        _fsm.animator.SetTrigger(_fsm.animIDClimb);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        timer += Time.deltaTime;
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private IEnumerator ClimbMovement()
    {
        while (timer < .17f)
        {
            _fsm.characterController.Move(Vector3.up * (0.6f / .17f) * Time.deltaTime);
            yield return null;
        }
        
        _fsm.Jump(1f);
 
        while (timer < .45f)
        {
            _fsm.characterController.Move(_fsm.transform.forward * (0.8f / .45f) * Time.deltaTime);
            yield return null;
        }
        
        fsm.RequestStateChange("Idle");
    }
}