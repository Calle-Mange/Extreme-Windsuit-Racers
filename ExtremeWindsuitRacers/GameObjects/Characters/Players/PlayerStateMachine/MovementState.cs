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
    [Export] protected float MinGlideSpeed = 50.0f;
    [Export] protected float MaxFallSpeed = 300.0f;
    [Export] protected float Acceleration = 10.0f;
    [Export] protected float FlySpeed = 50f;

    [ExportSubgroup("Steering Controll")]
    [Export] protected float MaxPitch = 89f;
    [Export] protected float MinPitch = -89f;
    [Export] protected float MouseSensitivity = 0.1f;
    [Export] protected float SmoothingFactor = 2f;

    protected Vector3 ForwardDirection;
    protected Vector3 TargetPosition;

    protected float CurrentSpeed;
    protected float AcceleratedSpeed;
    protected float targetPitch = 0.0f;
    protected float targetYaw = 0.0f;
    protected float currentPitch = 0.0f;
    protected float currentYaw = 0.0f;

    protected Tween RotationalTween;

    public override void _Ready() { }

	public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void StateReady()
    {
        ForwardDirection = Body.Transform.Basis.Z * -1;
    }

    public virtual void StateProcess(double delta) 
    {
        TargetPosition = Target.Position;
    }

    public virtual void StatePhysicsProcess(double delta) 
    {
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, (float)delta * SmoothingFactor);
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, (float)delta * SmoothingFactor);

        Body.Rotation = new Vector3(
            Mathf.DegToRad(currentPitch),
            Mathf.DegToRad(currentYaw),
            0f
        );

        ForwardDirection = Body.Transform.Basis.Z * -1;
    }

    /// <summary>
    /// Calculate the velocity for the state and return it.
    /// </summary>
    /// <param name="velocity">The velocity variable of the CharacterBody3D</param>
    /// <param name="delta">Delta from the physics process</param>
    /// <returns>Returns the velocity after calculating how the state should affect it</returns>
    public virtual Vector3 CalculateStateMovementVelocity(Vector3 velocity, double delta) 
    {
        //Add gravity, add acceleration, add deacceleration when pitch is above 0 etc.
        velocity = ForwardDirection * FlySpeed;
        return velocity;
    }

    public override void _Process(double delta) { }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            targetYaw -= motion.ScreenRelative.X * MouseSensitivity;
            targetPitch -= motion.ScreenRelative.Y * MouseSensitivity;

            targetPitch = Mathf.Clamp(targetPitch, MinPitch, MaxPitch);
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }
}
