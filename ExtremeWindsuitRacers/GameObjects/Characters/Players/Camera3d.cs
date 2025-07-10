using Godot;

public partial class Camera3d : Camera3D
{
	[Export(PropertyHint.Range, "0.0,10.0,0.01")]
	public float CameraOffset { get; set; } = 0.2f; // Offset to avoid clipping

	private RayCast3D _rayCast;
	private CharacterBody3D _glider;
	private Vector3 _relativeGliderPosition;
	private bool _wasColliding = false;

	public override void _Ready()
	{
		_rayCast = GetNode<RayCast3D>("RayCast3D");
		_glider = GetParent<CharacterBody3D>();
		_relativeGliderPosition = GlobalTransform.Origin - _glider.GlobalTransform.Origin;
	}

	public override void _Process(double delta)
	{
		if (_rayCast == null || _glider == null)
			return;

		bool isColliding = _rayCast.IsColliding();

		if (isColliding)
		{
			Vector3 collisionPoint = _rayCast.GetCollisionPoint();
			Vector3 collisionNormal = _rayCast.GetCollisionNormal();

			GlobalTransform = new Transform3D(
				GlobalTransform.Basis,
				collisionPoint + collisionNormal * CameraOffset
			);
		}
		else if (_wasColliding) // Only update when collision just stopped. Behöver flytta den till kamerans orginella position i relation till glidaren.
		{
			GlobalTransform = new Transform3D(
				GlobalTransform.Basis,
				_glider.GlobalTransform.Origin + _relativeGliderPosition
			);
		}

		_wasColliding = isColliding;
	}
}

//using Godot;

//public partial class Camera3d : Camera3D
//{
//	[Export(PropertyHint.Range, "0.0,10.0,0.01")]
//	public float CameraOffset { get; set; } = 0.2f; // Offset to avoid clipping

//	private RayCast3D _rayCast;
//	private CharacterBody3D _glider;
//	private Transform3D _relativeGliderTransform;
//	private bool _wasColliding = false;

//	public override void _Ready()
//	{
//		_rayCast = GetNode<RayCast3D>("RayCast3D");
//		_glider = GetParent<CharacterBody3D>();
//		// Store the camera's transform relative to the glider
//		_relativeGliderTransform = _glider.GlobalTransform.AffineInverse() * GlobalTransform;
//	}

//	public override void _Process(double delta)
//	{
//		if (_rayCast == null || _glider == null)
//			return;

//		bool isColliding = _rayCast.IsColliding();

//		if (isColliding)
//		{
//			Vector3 collisionPoint = _rayCast.GetCollisionPoint();
//			Vector3 collisionNormal = _rayCast.GetCollisionNormal();

//			GlobalTransform = new Transform3D(
//				GlobalTransform.Basis,
//				collisionPoint + collisionNormal * CameraOffset
//			);
//		}
//		else if (_wasColliding) // Only update when collision just stopped
//		{
//			// Restore both position and rotation relative to the glider
//			GlobalTransform = _glider.GlobalTransform * _relativeGliderTransform;
//		}

//		_wasColliding = isColliding;
//	}
//}