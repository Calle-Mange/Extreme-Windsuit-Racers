using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PlayerController : CharacterBody3D
{
    private float MaxSpeed = 8.0f;
    private float MinSpeed = 2.0f;
    private float Acceleration = 2.5f;
    private float Deacceleration = 1.5f;
    private float CurrentSpeed = 5.0f;

    private float yaw_speed = 45.0f;
    private float pitch_speed = 45.0f;
    private float roll_speed = 45.0f;

    private MeshInstance3D PlayerMesh;

    private Vector2 TurnInput;

    public override void _Ready()
    {
        PlayerMesh = GetNode<MeshInstance3D>("MeshInstance3D");

        yaw_speed = Mathf.DegToRad(yaw_speed);
        pitch_speed = Mathf.DegToRad(pitch_speed);
        roll_speed = Mathf.DegToRad(roll_speed);
    }

    public override void _PhysicsProcess(double delta)
	{
        Vector3 velocity = Velocity;

        Vector2 input = Input.GetVector("glide_left", "glide_right", "glide_up", "glide_down");
        float roll = Input.GetAxis("roll_left", "roll_right");

        if (input.Y > 0 && CurrentSpeed < MaxSpeed)
        {
            CurrentSpeed += Acceleration * (float)delta;
        }
        else if (input.Y < 0 && CurrentSpeed > MinSpeed)
        {
            CurrentSpeed -= Deacceleration * (float)delta;
        }

        velocity += -Basis.Z * CurrentSpeed;
        velocity += GetGravity() * (float)delta;   

        Velocity = velocity;
        MoveAndSlide();

        Vector3 turnDirection = new Vector3(-TurnInput.Y, -TurnInput.X, -roll);

        ApplyRotation(turnDirection, (float)delta);
        TurnInput = Vector2.Zero;
	}

    private void ApplyRotation(Vector3 turnDirection, double delta)
    {
        Rotate(Basis.Z, turnDirection.Z * roll_speed * (float)delta);
        Rotate(Basis.X, turnDirection.X * pitch_speed * (float)delta);
        Rotate(Basis.Y, turnDirection.Y * yaw_speed * (float)delta);

        if (turnDirection.Y < 0)
        {
            PlayerMesh.Rotation = new Vector3(PlayerMesh.Rotation.X, PlayerMesh.Rotation.Y, Mathf.LerpAngle(PlayerMesh.Rotation.Z, Mathf.DegToRad(-45f) * -turnDirection.Y, (float)delta));
        }
        else if (turnDirection.Y > 0)
        {
            PlayerMesh.Rotation = new Vector3(PlayerMesh.Rotation.X, PlayerMesh.Rotation.Y, Mathf.LerpAngle(PlayerMesh.Rotation.Z, Mathf.DegToRad(45f) * turnDirection.Y, (float)delta));
        }
        else
        {
            PlayerMesh.Rotation = new Vector3(PlayerMesh.Rotation.X, PlayerMesh.Rotation.Y, Mathf.LerpAngle(PlayerMesh.Rotation.Z, 0, (float)delta));
        }
    }

    private void OnMouseAnalogInput(Vector2 analog)
    {
        TurnInput = analog;
    }
}
