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
        //Accelerate towards target while pitch is lower than 0
        //Deaccelerate when pitch is higher than 0. Deacceleration is based on the pitch, with max pitch (90) reflecting full deacceleration at gravity speed.
        //At pitch 0, keep speed constant but no acceleration
        base.StatePhysicsProcess(delta);
    }

    public override Vector3 CalculateStateMovementVelocity(Vector3 velocity, double delta)
    {
        AcceleratedSpeed = CurrentSpeed + Acceleration;

        if (CurrentSpeed < MaxSpeed)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 5);
        }
        else
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (float)delta * 5);
        }
        velocity = ForwardDirection * CurrentSpeed;

        return velocity;
    }
}
