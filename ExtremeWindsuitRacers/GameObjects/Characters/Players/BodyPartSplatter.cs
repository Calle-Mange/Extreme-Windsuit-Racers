using Godot;
using System;
using System.Collections.Generic;

public partial class BodyPartSplatter : Node3D
{
	Marker3D SplatterCenter;
	List<RigidBody3D> BodyParts;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SplatterCenter = GetNode<Marker3D>("SplatterCenter");
		BodyParts = new List<RigidBody3D>();

		foreach (Node child in GetChildren())
		{
			if (child is RigidBody3D)
			{
				BodyParts.Add(child as RigidBody3D);
			}
		}

		foreach (RigidBody3D part in BodyParts)
		{
			part.ApplyForce(CalculateForceDirection(SplatterCenter.Position, part.Position) * 2000);
        }

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private Vector3 CalculateForceDirection(Vector3 center, Vector3 bodyPart)
	{
		Vector3 direction = bodyPart - center;

		return direction.Normalized();
	}
}
