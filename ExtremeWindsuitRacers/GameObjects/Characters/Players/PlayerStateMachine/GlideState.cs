using Godot;
using System;

public partial class GlideState : MovementState
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void Enter()
    {
        MaxPitch = 89f;
        MinPitch = -89f;
    }

    public override void Exit()
    {

    }

    public override void StateReady()
    {
        base.StateReady();
    }

    public override void StateProcess(double delta)
    {

    }

    public override void StatePhysicsProcess(double delta)
    {
        base.StatePhysicsProcess(delta);

        MovementStateMachine.AcceleratedSpeed = MovementStateMachine.CurrentSpeed + CalculateAcceleration(MovementStateMachine.Acceleration);

        MovementStateMachine.CurrentSpeed = Mathf.Lerp(MovementStateMachine.CurrentSpeed, MovementStateMachine.AcceleratedSpeed, (float)delta * 8);

        if (MovementStateMachine.CurrentSpeed > MaxSpeed)
        {
            MovementStateMachine.CurrentSpeed = Mathf.Lerp(MovementStateMachine.CurrentSpeed, MaxSpeed, (float)delta * 8);
        }
        if (MovementStateMachine.CurrentSpeed < MinSpeed)
        {
            MovementStateMachine.CurrentSpeed = Mathf.Lerp(MovementStateMachine.CurrentSpeed, MinSpeed, (float)delta * 8);
        }
    }
}
