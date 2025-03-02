using Godot;
using System;

public partial class WingSuitMomentumController : CharacterBody3D
{
    #region Constants
    const float YawRate = 4.0f;
    #endregion

    #region Exports
    [Export] float GravityCustom = 750f;
    [Export] float MaxSpeed = 100.0f;
    [Export] float MaxFallSpeed = -100.0f;
    [Export] float Acceleration = 5.0f;
    [Export] float MaxPitchAngleDegrees = 80.0f;
    [Export] float YawAnglePerSecond = 20.0f;
    #endregion

    #region Floats
    float CurrentSpeed = .0f;
    float MaxAcceleration;
    float AcceleratedSpeed;
    float PitchInput = .0f;
    float YawInput = .0f;
    float Yaw = .0f;
    #endregion

    #region Vectors
    Vector3 ForwardDirection;
    Vector2 TurnInput;
    #endregion

    #region Nodes
    MeshInstance3D PlayerMesh;
    Tween RotationalTween;
    #endregion

    public override void _Ready()
    {
        PlayerMesh = GetNode<MeshInstance3D>("MeshInstance3D");
        MaxAcceleration = Acceleration;
    }

    public override void _PhysicsProcess(double delta)
	{
        HandleInput();
        HandlePhysics(delta);
        HandleRotation(delta);
        HandleAnimation();
	}

    private void HandleInput()
    {
        PitchInput = TurnInput.Y * -1;
        YawInput = TurnInput.X * -1;
    }

    private void HandlePhysics(double delta)
    {
        Vector3 velocity = Velocity;
        ForwardDirection = Basis.Z * -1;

        AcceleratedSpeed = CurrentSpeed + (PitchInput * -Acceleration);

        if (PitchInput < 0)
        {
            Acceleration = (PitchInput / -1.0f) * MaxAcceleration;
            CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 8);
        }
        else if (PitchInput > 0)
        {
            Acceleration = (PitchInput / 1.0f) * MaxAcceleration;
            CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 5);
        }
        else
        {
            Acceleration = MaxAcceleration * 0f;
        }

        CurrentSpeed = Mathf.Clamp(CurrentSpeed, MaxFallSpeed, MaxSpeed);

        velocity = ForwardDirection * CurrentSpeed;
        velocity.Y -= GravityCustom * (float)delta;

        Velocity = velocity;
        MoveAndSlide();

    }

    private void HandleRotation(double delta)
    {
        if (YawInput != 0)
        {
            Yaw += YawInput;
        }
        else
        {
            Yaw = Mathf.Lerp(Yaw, 0, YawRate * (float)delta);
        }
        Yaw = Mathf.Clamp(Yaw, -YawAnglePerSecond, YawAnglePerSecond);
    }

    /// <summary>
    /// Handles rotation of player model.
    /// </summary>
    private void HandleAnimation()
    {
        if (RotationalTween != null)
        {
            RotationalTween.Kill();
        }

        RotationalTween = GetTree().CreateTween().SetEase(Tween.EaseType.Out).SetParallel(true);

        RotationalTween.TweenProperty(PlayerMesh, "rotation_degrees:z", YawInput * 45, .5f);

        if (PitchInput != 0)
        {
            RotationalTween.TweenProperty(this, "rotation_degrees:x", PitchInput * MaxPitchAngleDegrees, 1.25f);
        }

        RotationalTween.TweenProperty(this, "rotation_degrees:y", Yaw, 0.2f).AsRelative();
    }

    private void OnMouseAnalogInput(Vector2 analog)
	{
        TurnInput = analog;
    }
}
