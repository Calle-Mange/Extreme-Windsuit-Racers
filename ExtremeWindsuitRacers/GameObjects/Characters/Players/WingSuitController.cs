using Godot;
using System;

public partial class WingSuitController : CharacterBody3D
{
    #region Constants
    const float YawRate = 4.0f;
    #endregion

    #region Exports
    [Export] float GravityCustom = 10f;
    [Export] float MaxSpeed = 100.0f;
    [Export] float MinSpeed = 10.0f;
    [Export] float Acceleration = 5.0f;
    [Export] float MaxPitchAngleDegrees = 80.0f;
    [Export] float YawAnglePerSecond = 20.0f;
    [Export] float RiseMeterMax = 20.0f;
    #endregion

    #region Floats
    float CurrentSpeed = .0f;
    float RiseMeter = 0.0f;
    float RiseMeterMin;
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
        CurrentSpeed = MinSpeed;
        RiseMeterMin = RiseMeter;
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleInput();
        HandlePhysics(delta);
        HandleRotation(delta);
        HandleAnimation();
    }

    /// <summary>
    /// Handles input from player to determine pitch and yaw.
    /// </summary>
    private void HandleInput()
    {
        PitchInput = TurnInput.Y * -1;
        YawInput = TurnInput.X * -1;
    }

    /// <summary>
    /// Handles physics for the wingsuit movement. Rise meter determines how high player can go after building up speed downwards.
    /// </summary>
    /// <param name="delta">Delta used for calculating physics per second instead of frame.</param>
    private void HandlePhysics(double delta)
    {
        Vector3 velocity = Velocity;
        ForwardDirection = Basis.Z * -1;

        if (PitchInput < 0)
        {
            RiseMeter += 2 * (float)delta;
        }
        else if (PitchInput > 0)
        {
            if (RiseMeter > 0)
            {
                RiseMeter -= 2 * (float)delta;
            }
            else
            {
                PitchInput = Mathf.Lerp(PitchInput, 0, (YawRate * 0.25f) * (float)delta);
            }
        }
        RiseMeter = Mathf.Clamp(RiseMeter, RiseMeterMin, RiseMeterMax);

        AcceleratedSpeed = Mathf.Abs(CurrentSpeed + (PitchInput * -Acceleration));

        if (PitchInput < 0)
        {
            CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 6);
        }
        else
        {
            CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, CurrentSpeed / 4, (float)delta * 0.1f);
        }
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, MinSpeed, MaxSpeed);

        velocity = ForwardDirection * CurrentSpeed;
        velocity.Y -= GravityCustom * 10 * (float)delta;

        Velocity = velocity;
        MoveAndSlide();
    }

    /// <summary>
    /// Handles Yaw rotation to steer player left and right.
    /// </summary>
    /// <param name="delta">Delta used for calculating physics per second instead of frame.</param>
    private void HandleRotation(double delta)
    {
        if (YawInput != 0)
        {
            Yaw += YawInput;
        }
        else
        {
            Yaw = Mathf.Lerp(Yaw, 0, YawRate * (float)delta);
            GD.Print("Stop turning!");
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

        if (PitchInput < 0)
        {
            RotationalTween.TweenProperty(this, "rotation_degrees:x", PitchInput * MaxPitchAngleDegrees, .75f);
        }
        else if (PitchInput > 0)
        {
            RotationalTween.TweenProperty(this, "rotation_degrees:x", PitchInput * 0, .75f);
        }

        RotationalTween.TweenProperty(this, "rotation_degrees:y", Yaw, 0.2f).AsRelative();
    }

    /// <summary>
    /// Handles input from mouse to convert into TurnInput.
    /// </summary>
    /// <param name="analog">Mouse vector2 as calculated by the AnalogInputController.</param>
    private void OnMouseAnalogInput(Vector2 analog)
    {
        TurnInput = analog;
    }
}
