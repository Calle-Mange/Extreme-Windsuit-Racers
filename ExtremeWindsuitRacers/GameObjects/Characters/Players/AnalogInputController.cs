using Godot;
using System;
using System.Runtime.InteropServices;

public partial class AnalogInputController : Control
{
	private float Radius = 70f;
	private float DeadZone = 0.1f;

	Vector2 MousePosition;

	[Signal] public delegate void AnalogInputEventHandler(Vector2 analog);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
		Input.WarpMouse(Position);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		if (Input.MouseMode == Input.MouseModeEnum.Visible)
		{
			return;
		}

		Vector2 localMousePosition = GetLocalMousePosition();

		if (localMousePosition.Length() < Radius)
		{
			MousePosition = localMousePosition;
		}
		else
		{
			MousePosition = localMousePosition.Normalized() * Radius;
		}

		Input.WarpMouse(Position + MousePosition);

		Vector2 analog = new Vector2(MousePosition.X / Radius, -MousePosition.Y / Radius);

		if (analog.Length() > DeadZone)
		{
			EmitSignal(SignalName.AnalogInput, analog);
		}
		else
		{
			EmitSignal(SignalName.AnalogInput, new Vector2(0, analog.Y));
		}

		QueueRedraw();
	}

    public override void _Draw()
    {
		DrawArc(Vector2.Zero, Radius, 0, 360, 40, Colors.White, 5, true);
		DrawCircle(Vector2.Zero, DeadZone * 100, Colors.Red);
		DrawCircle(MousePosition, 10, Colors.White);
    }
}
