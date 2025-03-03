using Godot;
using System;

public partial class BodyPartExplosion : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		Vector3 ExplosionForce = new Vector3(-3000, -3000, -3000);
		ApplyForce(ExplosionForce);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
