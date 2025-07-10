using Godot;
using System;

public partial class Propeller : Node3D
{
	[Export] public float RotationSpeed = 5.0f; // Radians per second

	public override void _Process(double delta)
	{
		// Rotate around the local z-axis
		Rotation = new Vector3(
			Rotation.X,
			Rotation.Y,
			Rotation.Z + (float)(RotationSpeed * delta)
		);
	}
}