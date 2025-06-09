using Godot;
using System;
using System.Collections.Generic;

public partial class WingSuitMomentumController : CharacterBody3D
{
    #region Constants
    const float YawRate = 4.0f;
    #endregion

    #region Signals
    [Signal] public delegate void SignalPlayerDeathEventHandler();
    [Signal] public delegate void SignalPlayerRespawnEventHandler();
    #endregion

    #region Exports
    [Export] float GravityCustom = 1200f;
    [Export] float MaxSpeed = 200.0f;
    [Export] float MinGlideSpeed = 10.0f;
    [Export] float MaxFallSpeed = -1200.0f;
    [Export] float Acceleration = 10.0f;
    [Export] float MaxPitchAngleDegrees = 80.0f;
    [Export] float YawAnglePerSecond = 20.0f;
    #endregion

    #region Floats
    float CurrentSpeed = 1.0f;
    float MaxAcceleration;
    float AcceleratedSpeed;
    float PitchInput = .0f;
    float YawInput = .0f;
    float PitchDiveDirection = .0f;
    float Yaw = .0f;
    float RespawnSpeed;
    float RespawnAcceleration;
    float RespawnYaw;
    #endregion

    #region Vectors
    Vector3 ForwardDirection;
    Vector2 TurnInput;
    Vector3 RespawnPosition;
    #endregion

    #region Enums
    enum PlayerState
    {
        Gliding,
        Breaking,
        Diving,
        Dead
    }
    PlayerState CurrentPlayerState;
    #endregion

    #region Nodes
    MeshInstance3D PlayerMesh;
    Tween RotationalTween;
    CollisionShape3D PlayerCollider;
    AnimationTree PlayerAnimationTree;
    AnimationNodeStateMachinePlayback AnimationStateMachinePlayback;
    #endregion

    public override void _Ready()
    {
        PlayerMesh = GetNode<MeshInstance3D>("Armature/Skeleton3D/Body");
        PlayerCollider = GetNode<CollisionShape3D>("CollisionShape3D");
        PlayerAnimationTree = GetNode<AnimationTree>("AnimationTree");
        AnimationStateMachinePlayback = (AnimationNodeStateMachinePlayback)PlayerAnimationTree.Get("parameters/playback");
        MaxAcceleration = Acceleration;
        CurrentPlayerState = PlayerState.Gliding;

        RespawnPosition = Position;
        RespawnSpeed = CurrentSpeed;
        RespawnAcceleration = Acceleration;
        RespawnYaw = Yaw;
    }

    public override void _PhysicsProcess(double delta)
	{
        HandleInput();

        if (CurrentPlayerState != PlayerState.Dead)
        {
            HandlePhysics(delta);
            HandleCollision();
            HandleRotation(delta);
            HandleAnimation();
        }
        else
        {
            HandleRespawn();
        }
	}

    /// <summary>
    /// Handles input from player to determine pitch and yaw.
    /// </summary>
    private void HandleInput()
    {
        PitchInput = TurnInput.Y * -1;
        YawInput = TurnInput.X * -1;

        if (Input.IsActionJustPressed("dive"))
        {
            if (CurrentPlayerState == PlayerState.Diving)
            {
                CurrentPlayerState = PlayerState.Gliding;
            }
            else
            {
                CurrentPlayerState = PlayerState.Diving;
                AnimationStateMachinePlayback.Travel("dive_animation");
            }

            GD.Print(CurrentPlayerState);
        }

        if (Input.IsActionJustPressed("break"))
        {
            if (CurrentPlayerState == PlayerState.Breaking)
            {
                CurrentPlayerState = PlayerState.Gliding;
                AnimationStateMachinePlayback.Travel("flight_animation");
            }
            else
            {
                CurrentPlayerState = PlayerState.Breaking;
                AnimationStateMachinePlayback.Travel("break_animation");
            }

            GD.Print(CurrentPlayerState);
        }
    }

    /// <summary>
    /// Handles physics for the wingsuit movement. Rise meter determines how high player can go after building up speed downwards.
    /// </summary>
    /// <param name="delta">Delta used for calculating physics per second instead of frame.</param>
    private void HandlePhysics(double delta)
    {
        Vector3 velocity = Velocity;
        ForwardDirection = Basis.Z * -1;
        
        if (CurrentPlayerState == PlayerState.Gliding)
        {
            Velocity = HandleGliding(velocity, delta);
        }
        else if (CurrentPlayerState == PlayerState.Breaking)
        {
            Velocity = HandleBreaking(velocity, delta);
        }
        else if (CurrentPlayerState == PlayerState.Diving)
        {
            Velocity = HandleDiving(velocity, delta);
        }


        MoveAndSlide();
    }

    private Vector3 HandleGliding(Vector3 velocity, double delta)
    {
        AcceleratedSpeed = CurrentSpeed + (PitchInput * -Acceleration);

        if (CurrentSpeed > 0)
        {
            if (PitchInput < 0)
            {
                Acceleration = (PitchInput / -1.0f) * MaxAcceleration;
                CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 8);

                if (CurrentSpeed < MinGlideSpeed)
                {
                    CurrentSpeed = Mathf.Lerp(CurrentSpeed, MinGlideSpeed, (float)delta * 8);
                }
            }
            else if (PitchInput > 0)
            {
                if (CurrentSpeed > 0)
                {
                    Acceleration = (PitchInput / 1.0f) * MaxAcceleration;
                    CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 5);
                }
            }
            else
            {
                Acceleration = MaxAcceleration * 0f;
            }

            if (CurrentSpeed > MaxSpeed)
            {
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (float)delta * 8);
            }

            velocity = ForwardDirection * CurrentSpeed;
            velocity.Y -= GravityCustom * (float)delta;
        }
        else if (CurrentSpeed < 0)
        {
            velocity.Y -= (GravityCustom / 10) * (float)delta;
        }

        return velocity;
    }

    private Vector3 HandleDiving(Vector3 velocity, double delta)
    {
        //Limited pitch input to negative values. Remove ability to alter yaw.
        float maxDiveSpeed = MaxSpeed * 3;
        float maxDiveAcceleration = MaxAcceleration * 3;

        PitchInput = Mathf.Clamp(PitchInput, -0.95f, -0.75f);

        AcceleratedSpeed = CurrentSpeed + (PitchInput * -Acceleration);

        if (CurrentSpeed > 0)
        {
            if (PitchInput < 0)
            {
                Acceleration = (PitchInput / -1.0f) * maxDiveAcceleration;
                CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 8);
            }

            if (CurrentSpeed > maxDiveSpeed)
            {
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, maxDiveSpeed, (float)delta * 8);
            }

            velocity = ForwardDirection * CurrentSpeed;
            velocity.Y -= GravityCustom * (float)delta;
        }
        else if (CurrentSpeed < 0)
        {
            velocity.Y -= (GravityCustom / 10) * (float)delta;
        }

        return velocity;
    }

    private Vector3 HandleBreaking(Vector3 velocity, double delta)
    {
        //Reduce speed in opposite direction of forward until speed reaches 0, at which point the player will fall down. 
        //Activating glide or dive here gives a speedboost to get player started again

        AcceleratedSpeed = CurrentSpeed + (PitchInput * -Acceleration);
        float maxBreakSpeed = MaxSpeed * 0.05f;
        float maxBreakAcceleration = MaxAcceleration * 0.05f;
        PitchInput = Mathf.Clamp(PitchInput, -0.95f, 0.01f);

        if (CurrentSpeed > 0)
        {
            if (PitchInput < 0)
            {
                Acceleration = (PitchInput / -1.0f) * maxBreakAcceleration;
                CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 8);
            }
            else if (PitchInput > 0)
            {
                if (CurrentSpeed > 0)
                {
                    Acceleration = (PitchInput / 1.0f) * maxBreakAcceleration;
                    CurrentSpeed = (float)Mathf.Lerp(CurrentSpeed, AcceleratedSpeed, (float)delta * 3);
                }
            }
            else
            {
                Acceleration = maxBreakAcceleration * 0f;
            }

            if (CurrentSpeed > maxBreakSpeed)
            {
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, maxBreakSpeed, (float)delta * 5);
            }

            velocity = ForwardDirection * CurrentSpeed;
            velocity.Y -= GravityCustom * (float)delta;
        }
        else if (CurrentSpeed < 0)
        {
            velocity.Y -= (GravityCustom / 10) * (float)delta;
        }

        return velocity;
    }

    /// <summary>
    /// Handles Yaw rotation to steer player left and right.
    /// </summary>
    /// <param name="delta">Delta used for calculating physics per second instead of frame.</param>
    private void HandleRotation(double delta)
    {
        if (CurrentPlayerState != PlayerState.Diving)
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
        else
        {
            Yaw = 0;
        }

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

        RotationalTween.TweenProperty(PlayerMesh, "rotation_degrees:z", YawInput * -45, .5f);

        if (PitchInput != 0)
        {
            RotationalTween.TweenProperty(this, "rotation_degrees:x", PitchInput * MaxPitchAngleDegrees, 1.25f);
        }

        RotationalTween.TweenProperty(this, "rotation_degrees:y", Yaw, 0.2f).AsRelative();
    }

    /// <summary>
    /// Handles player collision with terrain. Effect of collision is determined by player speed.
    /// </summary>
    private void HandleCollision()
    {
        var collision = GetLastSlideCollision();

        if (collision != null)
        {
            var colliderType = collision.GetCollider();

            if (colliderType is not StaticBody3D)
            {
                return;
            }

            var colliderPosition = collision.GetPosition();

            if (CurrentSpeed < 0 || CurrentSpeed > 0)
            {
                var bodyPartsScene = GD.Load<PackedScene>("res://GameObjects/Characters/Players/PlayerBodyParts.tscn");
                var bodyParts = bodyPartsScene.Instantiate<Node3D>();
                GetParent().AddChild(bodyParts);
                bodyParts.Transform = Transform;

                PlayerCollider.SetProcess(false);
                PlayerMesh.Visible = false;
                CurrentPlayerState = PlayerState.Dead;
                EmitSignal(SignalName.SignalPlayerDeath);
            }
            else
            {
                GD.Print("Soft landing!");
            }
            
        }
    }

    private void HandleRespawn()
    {
        if (Input.IsActionJustPressed("respawn_player"))
        {
            PlayerMesh.Visible = true;
            Position = RespawnPosition;
            CurrentSpeed = RespawnSpeed;
            Acceleration = RespawnAcceleration;
            Yaw = RespawnYaw;
            AcceleratedSpeed = 5f;
            PlayerCollider.SetProcess(true);
            EmitSignal(SignalName.SignalPlayerRespawn);
      
            CurrentPlayerState = PlayerState.Gliding;
        }
    }

    private void OnMouseAnalogInput(Vector2 analog)
	{
        TurnInput = analog;
    }

    
}
