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
}
