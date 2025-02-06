using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	private Node3D FlightDirectionNode;
	public Vector3 FlightDirection;
	public float Acceleration;

    public override void _Ready()
    {
		FlightDirectionNode = GetNode<Node3D>("FlightDirection");
		Acceleration = 2;
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
        FlightDirection = FlightDirectionNode.Transform.Basis.Z * -1;

        // Add the gravity.
        if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		AngleFlightDirection(delta);

		velocity += FlightDirection * Acceleration;

		Velocity = velocity;
		MoveAndSlide();
	}

	/// <summary>
	/// This shit does not work yet.
	/// </summary>
	protected void AngleFlightDirection(double delta)
	{
		if (Input.IsActionPressed("glide_up"))
		{
			//FlightDirectionNode.Rotate(FlightDirectionNode.Transform.Basis.X.Normalized(), 10 * (float)delta);
			FlightDirectionNode.RotateX(-5 * (float)delta);
		}

        if (Input.IsActionPressed("glide_down"))
        {
            //FlightDirectionNode.Rotate(FlightDirectionNode.Transform.Basis.X.Normalized(), -10 * (float)delta);
            FlightDirectionNode.RotateX(5 * (float)delta);
        }

        if (Input.IsActionPressed("glide_right"))
        {
            //FlightDirectionNode.Rotate(FlightDirectionNode.Transform.Basis.Y.Normalized(), 10 * (float)delta);
            FlightDirectionNode.RotateY(5 * (float)delta);
        }

        if (Input.IsActionPressed("glide_left"))
        {
            //FlightDirectionNode.Rotate(FlightDirectionNode.Transform.Basis.Y.Normalized(), -10 * (float)delta);
            FlightDirectionNode.RotateY(-5 * (float)delta);
        }

		Vector3 FlightRotation = FlightDirectionNode.Rotation;
		FlightRotation.X = Mathf.Clamp(FlightRotation.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80));
		FlightRotation.Y = Mathf.Clamp(FlightRotation.Y, Mathf.DegToRad(-80f), Mathf.DegToRad(80));
		FlightDirectionNode.Rotation = FlightRotation;
    }
}
