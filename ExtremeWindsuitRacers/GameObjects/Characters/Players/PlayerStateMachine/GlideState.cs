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
        base.Enter();
    }

    public override void Exit()
    {

    }

    public override void StateReady()
    {
       
    }

    public override void StateProcess(double delta)
    {
        base.StateReady();
    }

    public override void StatePhysicsProcess(double delta)
    {

    }
}
