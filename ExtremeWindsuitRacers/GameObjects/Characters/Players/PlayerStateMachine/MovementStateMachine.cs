using Godot;
using System;
using System.Collections.Generic;

public partial class MovementStateMachine : Node
{
	[ExportSubgroup("Nodes")]
	[Export] public NodePath InitialState;
	[Export] public CharacterBody3D Body;

    #region Private State Variables
    private Dictionary<string, MovementState> States;
	private MovementState CurrentState;
    #endregion

    #region Public Movement Variables
    public float CurrentSpeed;
    public float Acceleration;
    public float AcceleratedSpeed;
    public float currentPitch = 0.0f;
    public float currentYaw = 0.0f;
    public float AcceleratedGravity;
    public float CurrentGravitySpeed;
    public float targetPitch = 0.0f;
    public float targetYaw = 0.0f;
    public float lastPitch { get; protected set; }
    public float lastYaw { get; protected set; }
    protected Tween RotationalTween;
    #endregion

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
                GD.Print("Changing to glide");
            }
			else
			{
                TransitionTo("DiveState");
                GD.Print("Changing to dive");
            }
        }

        if (Input.IsActionJustPressed("break"))
        {
            if (CurrentState == States["BreakState"])
            {
                TransitionTo("GlideState");
				GD.Print("Changing to glide");
            }
            else
            {
                TransitionTo("BreakState");
                GD.Print("Changing to break");
            }
        }
    }
}
