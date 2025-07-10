using Godot;
using System;

public partial class Glider : CharacterBody3D
{
	public bool _gliderCrashed = false;
	public Area3D _gliderHitbox;

	public bool GliderCrashed
	{
		get => _gliderCrashed;
		set => _gliderCrashed = value;
	}

	public Transform3D GliderStartPosition
	{
		get => _gliderStartPosition;
	}

	private Transform3D _gliderStartPosition;

	public override void _Ready()
	{
		Visible = true;
		_gliderStartPosition = GlobalTransform;
		_gliderHitbox = GetNode<Area3D>("GliderHitbox");
		_gliderHitbox.BodyEntered += OnGliderCollision;
	}

	public void RespawnGlider()
	{
		GD.Print("Respawning Glider...");
		GlobalTransform = _gliderStartPosition;
		Visible = true;
		_gliderCrashed = false;
	}

	private void OnGliderCollision(Node body)
	{
		GD.Print($"Glider collided with: {body.Name} (Type: {body.GetType().Name})");
		GD.Print("Glider collided! Respawning...");
		Visible = false;
		_gliderCrashed = true;
	}
}
