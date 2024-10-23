using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();

        // Debug.Log("hello?");
    }

    public override void Update()
    {
        // Debug.Log("hello?");
        base.Update();

        if (xInput == player.facingDir && player.isWallDetected())
            return;

        if (xInput != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
