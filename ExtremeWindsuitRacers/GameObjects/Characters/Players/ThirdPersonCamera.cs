using Godot;
using System.Runtime.InteropServices;

public partial class ThirdPersonCamera : Camera3D
{
	[DllImport("kernel32.dll")]
	static extern bool AllocConsole();

	private CharacterBody3D _glider;
	private Area3D _cameraInnerHitBox;
	private Area3D _cameraOuterHitBox;
	private CollisionShape3D _cameraHitBoxInnerArea;
	private CollisionShape3D _cameraHitBoxOuterArea;
	private Node3D _gliderCollisionShape;
	private Camera3D _birdCamera;
	private Camera3D _bodyCamera;
	private Camera3D _faceCamera;

	// Transition state
	private bool _isTransitioning = false;
	private float _transitionDuration = 2.5f; // default seconds
	private float _transitionProgress = 0f;
	private Transform3D _startTransform;
	private Camera3D _transitionTargetCamera;
	private Camera3D _previousCamera;

	// Thresholds for ending the transition
	private const float PositionThreshold = 0.01f;

	// Custom transition durations
	private const float DefaultTransitionDuration = 2.5f;
	private const float MediumTransitionDuration = 10.0f; 
	private const float SlowTransitionDuration = 50.0f; 

	// Helper properties for collision
	private bool IsCameraInnerColliding => _cameraInnerHitBox?.GetOverlappingBodies().Count > 0;
	private bool IsCameraOuterColliding => _cameraOuterHitBox?.GetOverlappingBodies().Count > 0;

	public override void _Ready()
	{
		AllocConsole();
		_glider = GetParent<CharacterBody3D>();
		_gliderCollisionShape = _glider.GetNode<Node3D>("GliderCollisionShape3D");
		_birdCamera = _glider.GetNode<Camera3D>("BirdCamera");
		_bodyCamera = _glider.GetNode<Camera3D>("BodyCamera");
		_faceCamera = _glider.GetNode<Camera3D>("FaceCamera");
		_cameraInnerHitBox = _glider.GetNode<Area3D>("CameraInnerHitBox");
		_cameraOuterHitBox = _glider.GetNode<Area3D>("CameraOuterHitBox");
		_cameraHitBoxInnerArea = _cameraInnerHitBox.GetNode<CollisionShape3D>("CameraInnerCollisionShape");
		_cameraHitBoxOuterArea = _cameraOuterHitBox.GetNode<CollisionShape3D>("CameraOuterCollisionShape");

		_isTransitioning = false;
		_transitionTargetCamera = null;
		_previousCamera = null;
		Current = true;
	}

	private void StartTransition(Camera3D targetCamera, float? customDuration = null)
	{
		_isTransitioning = true;
		_transitionProgress = 0f;
		_startTransform = GlobalTransform;
		_previousCamera = _transitionTargetCamera;
		_transitionTargetCamera = targetCamera;
		_transitionDuration = customDuration ?? DefaultTransitionDuration;
	}

	public override void _Process(double delta)
	{
		if (_cameraInnerHitBox == null || _cameraOuterHitBox == null || _glider == null || _gliderCollisionShape == null ||
			_bodyCamera == null || _faceCamera == null || _birdCamera == null)
			return;

		// Smooth transition logic
		if (_isTransitioning && _transitionTargetCamera != null)
		{
			Transform3D gliderShapeTransform = _gliderCollisionShape.GlobalTransform;
			Transform3D targetTransform = _transitionTargetCamera.GlobalTransform;
			Transform3D relative = gliderShapeTransform.AffineInverse() * targetTransform;
			Transform3D currentTargetTransform = gliderShapeTransform * relative;

			_transitionProgress += (float)delta / _transitionDuration;
			float t = Mathf.Clamp(_transitionProgress, 0f, 1f);

			var interpBasis = GlobalTransform.Basis.Slerp(currentTargetTransform.Basis, t);
			var interpOrigin = GlobalTransform.Origin.Lerp(currentTargetTransform.Origin, t);
			GlobalTransform = new Transform3D(interpBasis, interpOrigin);

			bool positionClose = GlobalTransform.Origin.DistanceTo(currentTargetTransform.Origin) < PositionThreshold;
			bool rotationClose = GlobalTransform.Basis.IsEqualApprox(currentTargetTransform.Basis);

			if (positionClose && rotationClose)
			{
				_isTransitioning = false;
				GlobalTransform = currentTargetTransform;
			}
		}

		// Priority 1: If inner hitbox is colliding (regardless of outer), always go to FaceCamera
		if (IsCameraInnerColliding)
		{
			if (_transitionTargetCamera != _faceCamera || !_isTransitioning)
			{
				GD.Print("Inner hitbox (or both hitboxes) colliding. Forcing transition to FaceCamera.");
				StartTransition(_faceCamera, DefaultTransitionDuration);
			}
			return;
		}

		// Priority 2: If only outer hitbox is colliding, go to BodyCamera
		if (IsCameraOuterColliding && !IsCameraInnerColliding)
		{
			if (_transitionTargetCamera != _bodyCamera || !_isTransitioning)
			{
				GD.Print("Only outer hitbox colliding. Forcing transition to BodyCamera.");
				StartTransition(_bodyCamera, MediumTransitionDuration);
			}
			return;
		}

		// Priority 3: No collisions (lowest, do not abort ongoing transitions)
		if (!_isTransitioning && _transitionTargetCamera != _birdCamera)
		{
			GD.Print("No collisions. Transitioning to BirdCamera.");
			StartTransition(_birdCamera, SlowTransitionDuration);
		}
	}
}