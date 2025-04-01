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
        //Lock pitch to -90 and remove ability to steer.
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
        //Lerp towards target and boost acceleration the closer to the target that you get.
        base.StatePhysicsProcess(delta);
    }

    public override Vector3 CalculateStateMovementVelocity(Vector3 velocity, double delta)
    {
        return velocity;
    }
}
