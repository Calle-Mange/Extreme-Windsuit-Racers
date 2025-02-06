using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	public Vector3 FlightDirection;
	public float Acceleration;

    public override void _Ready()
    {
		Acceleration = 1;
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
        FlightDirection = Transform.Basis.Z * -1;

        // Add the gravity.
        if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		AngleFlightDirection(delta);

		velocity += FlightDirection * Acceleration;
		velocity = velocity.Clamp(-30, 30);

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
			RotateX(-5 * (float)delta);
		}

        if (Input.IsActionPressed("glide_down"))
        {
			RotateX(5 * (float)delta);
        }

        if (Input.IsActionPressed("glide_right"))
        {
			RotateY(-5 * (float)delta);
        }

        if (Input.IsActionPressed("glide_left"))
        {
			RotateY(5 * (float)delta);
        }

		Vector3 FlightRotation = Rotation;
		Rotation = FlightRotation;
    }
}
