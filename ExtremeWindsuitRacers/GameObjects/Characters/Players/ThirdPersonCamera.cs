using Godot;

public partial class ThirdPersonCamera : Camera3D
{
	private RayCast3D _raycolider;
	private Vector3 _originalRelativePosition;
	[Export] public float CollisionYOffset { get; set; } = 1.0f; // Amount to lower the camera on collision

	public override void _Ready()
	{
		_raycolider = GetNode<RayCast3D>("camcolider");
		_originalRelativePosition = Position; // Store the camera's original relative position
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_raycolider == null)
			return;

		if (_raycolider.IsColliding())
		{
			Vector3 collisionPoint = _raycolider.GetCollisionPoint();
			collisionPoint.Y -= CollisionYOffset;
			GlobalTransform = new Transform3D(GlobalTransform.Basis, collisionPoint);
		}
		else
		{
			// Restore the camera's original relative position
			Position = _originalRelativePosition;
		}
	}
}