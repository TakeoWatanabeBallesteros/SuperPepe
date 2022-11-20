using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class Run : StateBase
{
    private PlayerFSM playerFsm;
    
    public Run(PlayerFSM fsm) : base(needsExitTime: false)
    {
        this.playerFsm = fsm;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnLogic()
    {
        playerFsm.currentSpeed = Mathf.Lerp(playerFsm.currentSpeed, playerFsm.runSpeed, Time.deltaTime * 10);
        
        Vector3 inputDirection = playerFsm.transform.right * playerFsm.MoveInput.x + playerFsm.transform.forward * playerFsm.MoveInput.y;
        
        playerFsm.collisionFlags =  playerFsm.controller.Move(inputDirection.normalized * (playerFsm.currentSpeed * Time.deltaTime) +
                                                              new Vector3(0.0f, playerFsm.verticalVelocity, 0.0f) * Time.deltaTime);
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
