using Godot;
using System;

public partial class DiveState : MovementState
{
	private AnimationTree _animationTree;

	public override void StateReady()
	{
		base.StateReady();
		// Get the AnimationTree node from the Glider (Body)
		_animationTree = Body.GetNode<AnimationTree>("AnimationTree");
	}

	public override void Enter()
	{
        base.Enter();

        if (_animationTree != null)
		{
			var playback = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
			playback.Travel("dive_animation");
		}
	}

	public override void Exit()
	{

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