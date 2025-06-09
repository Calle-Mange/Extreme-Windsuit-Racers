using Godot;
using System;
using static Godot.TextServer;

public partial class MovementState : Node
{
    public MovementStateMachine MovementStateMachine;
    public CharacterBody3D Body;
    public Node3D Target;

    [ExportSubgroup("Speed Controll")]
    [Export] protected float MaxSpeed = 200.0f;
    [Export] protected float MinSpeed = 0f;
    [Export] protected float MinGlideSpeed = 50.0f;
    [Export] protected float MaxFallSpeed = 300.0f;
    [Export] protected float MaxAcceleration = 10.0f;
    [Export] protected float MaxDeacceleration = -10.0f;

    [ExportSubgroup("Steering Controll")]
    [Export] protected float MaxPitch = 89f;
    [Export] protected float MinPitch = -89f;
    [Export] protected float MouseSensitivity = 0.1f;
    [Export] protected float SmoothingFactor = 2f;

    protected Vector3 ForwardDirection;
    protected float gravity = 982f;

    public override void _Ready() { }

    /// <summary>
    /// Method called when the state is entered.
    /// </summary>
    public virtual void Enter() 
    {

    }

    /// <summary>
    /// Method called when the state is exited. 
    /// </summary>
    public virtual void Exit() 
    {

    }

    /// <summary>
    /// Method that is called at the beginning of the runtime as the state machine is initiated.
    /// </summary>
    public virtual void StateReady()
    {
        ForwardDirection = Body.Transform.Basis.Z * -1;
    }

    /// <summary>
    /// Method for non-physics processes and caulculations for the state.
    /// </summary>
    /// <param name="delta"></param>
    public virtual void StateProcess(double delta)
    {

    }

    /// <summary>
    /// Method for physics processes and calculations for the state.
    /// </summary>
    /// <param name="delta"></param>
    public virtual void StatePhysicsProcess(double delta)
    {
        MovementStateMachine.currentPitch = Mathf.Lerp(MovementStateMachine.currentPitch, MovementStateMachine.targetPitch, (float)delta * SmoothingFactor);
        MovementStateMachine.currentYaw = Mathf.Lerp(MovementStateMachine.currentYaw, MovementStateMachine.targetYaw, (float)delta * SmoothingFactor);

        Body.Rotation = new Vector3(
            Mathf.DegToRad(MovementStateMachine.currentPitch),
            Mathf.DegToRad(MovementStateMachine.currentYaw),
            0f
        );

        ForwardDirection = Body.Transform.Basis.Z * -1;
    }

    /// <summary>
    /// Calculate the velocity for the state and return it.
    /// </summary>
    /// <param name="velocity">The velocity variable of the CharacterBody3D.</param>
    /// <param name="delta">Delta from the physics process.</param>
    /// <returns>Returns the velocity after calculating how the state should affect it.</returns>
    public virtual Vector3 CalculateStateMovementVelocity(Vector3 velocity, double delta)
    {
        velocity = ForwardDirection * MovementStateMachine.CurrentSpeed;
        velocity.Y -= gravity * (float)delta;
        return velocity;
    }

    /// <summary>
    /// Calculate the acceleration based on the current pitch input.
    /// </summary>
    /// <param name="acceleration">The acceleration variable that should be calculated.</param>
    /// <returns>Return the calculated acceleration.</returns>
    protected virtual float CalculateAcceleration(float acceleration)
    {
        acceleration = (MovementStateMachine.currentPitch / MaxPitch) * MaxAcceleration * -1;
        acceleration = Mathf.Clamp(acceleration, MaxDeacceleration, MaxAcceleration);

        return acceleration;
    }

    /// <summary>
    /// Calculate the gravity acceleration.
    /// </summary>
    /// <param name="gravityAcceleration">The gravity acceleration variable that should be calculated.</param>
    /// <returns>Return the calculated gravity acceleration.</returns>
    protected virtual float CalculateGravityAcceleration(float gravityAcceleration)
    {
        return gravityAcceleration;
    }

    public override void _Process(double delta) { }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            MovementStateMachine.targetYaw -= motion.ScreenRelative.X * MouseSensitivity;
            MovementStateMachine.targetPitch -= motion.ScreenRelative.Y * MouseSensitivity;

            MovementStateMachine.targetPitch = Mathf.Clamp(MovementStateMachine.targetPitch, MinPitch, MaxPitch);
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }
}
