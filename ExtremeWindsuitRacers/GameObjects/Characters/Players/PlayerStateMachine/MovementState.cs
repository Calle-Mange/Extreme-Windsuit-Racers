using Godot;
using System;

public partial class MovementState : Node
{
	public MovementStateMachine MovementStateMachine;
    public CharacterBody3D Body;

    [ExportSubgroup("Speed Controll")]
    [Export] protected float MaxSpeed = 200.0f;
    [Export] protected float MinGlideSpeed = 50.0f;
    [Export] protected float MaxFallSpeed = 300.0f;
    [Export] protected float Acceleration = 10.0f;

    [ExportSubgroup("Steering Controll")]
    [Export] protected float MaxPitch = 90;
    [Export] protected float MinPitch = -90;
    [Export] protected float MaxYaw = 90;
    [Export] protected float MinYaw = -90;

    protected Vector3 ForwardDirection;
    protected Vector3 TargetPosition;

    protected float CurrentSpeed;
    protected float AcceleratedSpeed;

    protected Tween RotationalTween;

    public override void _Ready() { }

	public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void StateReady()
    {
        ForwardDirection = Body.Basis.Z * -1;
    }

    public virtual void StateProcess(double delta) { }

    public virtual void StatePhysicsProcess(double delta) 
    {
        //Manipulate target which should affect where the character aims towards
        TargetPosition = MovementStateMachine.Target.GlobalPosition;
        GD.Print(TargetPosition);
        RotateTowardsTarget();
    }

    /// <summary>
    /// Calculate the velocity for the state and return it.
    /// </summary>
    /// <param name="velocity">The velocity variable of the CharacterBody3D</param>
    /// <param name="delta">Delta from the physics process</param>
    /// <returns>Returns the velocity after calculating how the state should affect it</returns>
    public virtual Vector3 CalculateStateMovementVelocity(Vector3 velocity, double delta) 
    {
        return velocity;
    }

    public override void _Process(double delta) { }

    /// <summary>
    /// Rotates the character towards the target.
    /// </summary>
    protected virtual void RotateTowardsTarget()
    {
        //Get latest target position and slerp currentTargetPosition towards the target
        //Use LookAt to rotate towards the currentTargetPosition
        var rotation = Body.Quaternion;
        Body.LookAt(TargetPosition, Vector3.Up);
        var targetRotation = Body.Quaternion;
        Body.Rotation = rotation.Slerp(targetRotation, 1).GetEuler();
    }

}
