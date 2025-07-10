using Godot;
using System;

public partial class FlightTestLevelFelix : Node3D
{
	private Glider _glider;
	private Control _respawnMenu;
	public bool _gliderCrashed = false;

	public bool GliderCrashed
	{
		get => _gliderCrashed;
		set => _gliderCrashed = value;
	}

	public override void _Ready()
	{
		_glider = GetNode<Glider>("Glider");

		var gliderCollisionBox = _glider.GetNode<Area3D>("GliderHitbox");
		gliderCollisionBox.BodyEntered += OnGliderCollision;

		_respawnMenu = GetNode<Control>("Respawn_Menu");
		_respawnMenu.Visible = false; // Hide menu at start
	}

	public void RespawnGlider()
	{
		_glider.RespawnGlider();
	}

	private void OnGliderCollision(Node body)
	{
		_respawnMenu.Visible = true;
	}
}
