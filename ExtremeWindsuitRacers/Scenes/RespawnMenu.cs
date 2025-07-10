using Godot;
using System;

public partial class RespawnMenu : Control
{
	public override void _Ready()
	{
		var respawnButton = GetNode<Button>("MarginContainer/VBoxContainer/Restart");
		respawnButton.Pressed += OnRespawnButtonPressed;
	}

	private void OnRespawnButtonPressed()
	{
		var level = GetParent<FlightTestLevelFelix>();
		if (level != null)
		{
			level.RespawnGlider();
			Visible = false;
		}
	}
}