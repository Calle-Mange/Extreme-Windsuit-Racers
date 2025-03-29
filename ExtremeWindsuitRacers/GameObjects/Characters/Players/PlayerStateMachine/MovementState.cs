using Godot;
using System;

public partial class MovementState : Node
{
	public MovementStateMachine MovementStateMachine;

    [Export] protected float MaxSpeed = 200.0f;
    [Export] protected float MinGlideSpeed = 50.0f;
    [Export] protected float MaxFallSpeed = -1200.0f;
    [Export] protected float Acceleration = 10.0f;

    public override void _Ready() { }

	public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void StateReady(){ }

    public virtual void StateProcess(double delta) { }

    public virtual void StatePhysicsProcess(double delta) { }

    public override void _Process(double delta) { }

}
