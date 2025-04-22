using Godot;
using System;
using System.Collections.Generic;

public partial class MovementStateMachine : Node
{
	[ExportSubgroup("Nodes")]
	[Export] public NodePath InitialState;
	[Export] public CharacterBody3D Body;

	private Dictionary<string, MovementState> States;
	private MovementState CurrentState;

	public override void _Ready()
	{
		States = new Dictionary<string, MovementState>();

		foreach (Node node in GetChildren())
		{
			if (node is MovementState state)
			{
				States[node.Name] = state;
				state.MovementStateMachine = this;
				state.Body = Body;
				state.StateReady();
				state.Exit();
			}
		}

		CurrentState = GetNode<MovementState>(InitialState);
		CurrentState.Enter();

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _Process(double delta)
	{
		HandleInput();
        CurrentState.StateProcess(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		CurrentState.StatePhysicsProcess(delta);
		Body.Velocity = CurrentState.CalculateStateMovementVelocity(Body.Velocity, delta);
		Body.MoveAndSlide();
	}

	public void TransitionTo(string state)
	{
		if (!States.ContainsKey(state) || CurrentState == States[state])
		{
			return;
		}

		CurrentState.Exit();
        CurrentState = States[state];
		CurrentState.Enter();
    }

	protected void HandleInput()
	{
        if (Input.IsActionJustPressed("dive"))
        {
			if (CurrentState == States["DiveState"])
			{
				TransitionTo("GlideState");
			}
			else
			{
                TransitionTo("DiveState");
            }
        }

        if (Input.IsActionJustPressed("break"))
        {
            if (CurrentState == States["BreakState"])
            {
                TransitionTo("GlideState");
            }
            else
            {
                TransitionTo("BreakState");
            }
        }
    }
}
