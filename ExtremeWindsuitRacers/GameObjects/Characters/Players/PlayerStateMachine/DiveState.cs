using Godot;
using System;

public partial class DiveState : MovementState
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

        AcceleratedSpeed = CurrentSpeed + CalculateAcceleration(Acceleration);

        CurrentSpeed = Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 8);

        if (CurrentSpeed > MaxSpeed)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (float)delta * 8);
        }
        if (CurrentSpeed < MinSpeed)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MinSpeed, (float)delta * 8);
        }
    }
}
