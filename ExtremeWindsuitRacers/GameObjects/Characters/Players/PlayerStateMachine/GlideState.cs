using Godot;
using System;

public partial class GlideState : MovementState
{
	private AnimationTree _animationTree;
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
        base.StateReady();
		// Get the AnimationTree node from the Glider (Body)
		_animationTree = Body.GetNode<AnimationTree>("AnimationTree");
	}

    public override void StateProcess(double delta)
    {
		// Play the "flight_animation" state in the AnimationTree
		if (_animationTree != null)
		{
			// Assuming you use a state machine in the AnimationTree
			var playback = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
			playback.Travel("flight_animation");
		}
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
