using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Jump : StateBase
{
    private PlayerFSM playerFsm;
    
    public Jump(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this.playerFsm = fsm;
    }

    public override void OnEnter()
    {
        playerFsm._verticalVelocity = Mathf.Sqrt(playerFsm.jumpHeight * -2f * playerFsm.gravity);
        base.OnEnter();
    }

    public override void OnLogic()
    {
        playerFsm._verticalVelocity += playerFsm.gravity * Time.deltaTime;
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
