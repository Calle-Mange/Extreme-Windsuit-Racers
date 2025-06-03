using Godot;
using System;
using static Godot.DisplayServer;

public partial class Movement : CharacterBody3D
{
	[Export]
	public float FlySpeed { get; set; } = 10.0f;

	[Export]
	public float MouseSensitivity { get; set; } = 0.1f;

	private float _yaw = 0.0f;
	private float _pitch = 0.0f;

	public override void _Ready()
	{
		GD.Print("ThirdPersonCamera _Ready called");
		Input.MouseMode = (Input.MouseModeEnum)MouseMode.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			_yaw -= mouseMotion.Relative.X * MouseSensitivity;
			_pitch -= mouseMotion.Relative.Y * MouseSensitivity;
			_pitch = Mathf.Clamp(_pitch, -89, 89); // Prevent flipping
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Apply rotation from mouse
		Rotation = new Vector3(
			Mathf.DegToRad(_pitch),
			Mathf.DegToRad(_yaw),
			0
		);

		// Always move forward in local -Z direction
		Velocity = -Transform.Basis.Z * FlySpeed;
		MoveAndSlide();

		// Check for collisions
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			//if (collision != null)
			//{
			//	// Restart the current scene
			//	var tree = GetTree();
			//	tree.ReloadCurrentScene();
			//	break;
			//}
		}
	}
}