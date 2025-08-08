using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

public partial class MovementStateMachine : Node
{
	[ExportSubgroup("Nodes")]
	[Export] public NodePath InitialState;
	[Export] public CharacterBody3D Body;

	#region Private State Variables
	private Dictionary<string, MovementState> States;
	private MovementState CurrentState;
	private Glider _glider;
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
	public float CurrentMaxPitch = 89f;
	public float CurrentMinPitch = -89f;
    protected Tween RotationalTween;
	#endregion

	public override void _Ready()
	{
		States = new Dictionary<string, MovementState>();
		_glider = GetParent<Glider>();

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

		var gliderCollisionBox = _glider.GetNode<Area3D>("GliderHitbox");
		gliderCollisionBox.BodyEntered += OnGliderCollision;

		CurrentState = GetNode<MovementState>(InitialState);
		CurrentState.Enter();

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	private void OnGliderCollision(Node body)
	{
		ResetMovement();
	}

	public override void _Process(double delta)
	{
		HandleInput();
		//GD.Print(CurrentSpeed);

		CurrentState.StateProcess(delta);

	}

	public void ResetMovement()
	{
		// Remove and free all MovementState children
		foreach (var stateName in new[] { "GlideState", "DiveState", "BreakState" })
		{
			var oldState = GetNodeOrNull<MovementState>(stateName);
			if (oldState != null)
			{
				RemoveChild(oldState);
				oldState.QueueFree();
			}
		}

		// Clear the States dictionary
		States.Clear();

		// Re-instantiate and add new state nodes
		var glideState = new GlideState();
		glideState.Name = "GlideState";
		AddChild(glideState);

		var diveState = new DiveState();
		diveState.Name = "DiveState";
		AddChild(diveState);

		var breakState = new BreakState();
		breakState.Name = "BreakState";
		AddChild(breakState);

		// Re-initialize and register states
		foreach (Node node in GetChildren())
		{
			if (node is MovementState state)
			{
				States[state.Name] = state;
				state.MovementStateMachine = this;
				state.Body = Body;
				state.StateReady();
				state.Exit();
			}
		}

		// Set the current state to the initial state
		CurrentState = GetNode<MovementState>(InitialState);
		CurrentState.Enter();

		// Reset movement variables as needed
		CurrentSpeed = 0f;
		Acceleration = 0f;
		AcceleratedSpeed = 0f;
		currentPitch = 0.0f;
		currentYaw = 0.0f;
		AcceleratedGravity = 0f;
		CurrentGravitySpeed = 0f;
		targetPitch = 0.0f;
		targetYaw = 0.0f;
		RotationalTween = null;

	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_glider.GliderCrashed == true)
		{
			CurrentState.StatePhysicsProcess(delta);
			Body.Velocity = CurrentState.CalculateStateMovementVelocity(Body.Velocity, delta);
			Body.MoveAndSlide();
		}
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
