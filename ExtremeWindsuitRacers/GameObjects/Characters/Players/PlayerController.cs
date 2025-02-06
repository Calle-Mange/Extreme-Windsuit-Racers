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
		FlightDirection = Transform.Basis.Z * -1;
		Acceleration = 1;
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

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

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("walk_left", "walk_right", "walk_forward", "walk_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
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
			FlightDirection = FlightDirection.Rotated(Transform.Basis.X.Normalized(), 10 * (float)delta);
		}

        if (Input.IsActionPressed("glide_down"))
        {
            FlightDirection = FlightDirection.Rotated(Transform.Basis.X.Normalized(), -10 * (float)delta);
        }

        if (Input.IsActionPressed("glide_right"))
        {
            FlightDirection = FlightDirection.Rotated(Transform.Basis.Y.Normalized(), 10 * (float)delta);
        }

        if (Input.IsActionPressed("glide_left"))
        {
            FlightDirection = FlightDirection.Rotated(Transform.Basis.Y.Normalized(), -10 * (float)delta);
        }
    }
}
