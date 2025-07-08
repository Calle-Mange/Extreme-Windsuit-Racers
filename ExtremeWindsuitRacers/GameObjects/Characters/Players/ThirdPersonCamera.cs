using Godot;
using System.Runtime.InteropServices;

public partial class ThirdPersonCamera : Camera3D
{
	[DllImport("kernel32.dll")]
	static extern bool AllocConsole();

	private RayCast3D _rayCast;
	private CharacterBody3D _glider;
	private Camera3D _camera3d2;
	private RayCast3D _rayCast3d2;
	private Camera3D _camera3d3;
	private bool _hasCollisionBeenReleased;
	private bool _hasCamera3d3BeenReleased;

	public override void _Ready()
	{
		AllocConsole();
		_rayCast = GetNode<RayCast3D>("RayCast3D");
		_glider = GetParent<CharacterBody3D>();
		_camera3d2 = _glider.GetNode<Camera3D>("Camera3D2");
		_rayCast3d2 = _camera3d2.GetNode<RayCast3D>("RayCast3D2");
		_camera3d3 = _glider.GetNode<Camera3D>("Camera3D3");

		_hasCollisionBeenReleased = true;
		_hasCamera3d3BeenReleased = true;
		Current = true;
		if (_camera3d2 != null) _camera3d2.Current = false;
		if (_camera3d3 != null) _camera3d3.Current = false;
	}

	public override void _Process(double delta)
	{
		if (_rayCast == null || _glider == null || _camera3d2 == null || _rayCast3d2 == null || _camera3d3 == null)
			return;

		// First switch: ThirdPersonCamera <-> Camera3D2
		if (_rayCast.IsColliding() && _hasCollisionBeenReleased)
		{
			GD.Print("Camera is colliding with an object. Switching to Camera3D2.");
			Current = false;
			_camera3d2.Current = true;
			_camera3d3.Current = false;
			_hasCollisionBeenReleased = false;
			_hasCamera3d3BeenReleased = true; // Reset for next stage
		}
		else if (!_rayCast.IsColliding() && !_hasCollisionBeenReleased)
		{
			GD.Print("Collision has been released. Switching back to ThirdPersonCamera.");
			Current = true;
			_camera3d2.Current = false;
			_camera3d3.Current = false;
			_hasCollisionBeenReleased = true;
			_hasCamera3d3BeenReleased = true;
		}

		// Second switch: Camera3D2 <-> Camera3D3 (RayCast3D2)
		// Switch to Camera3D3 if Camera3D2 is active and RayCast3D2 collides
		if (_camera3d2.Current && _rayCast3d2.IsColliding() && _hasCamera3d3BeenReleased)
		{
			GD.Print("RayCast3D2 is colliding. Switching to Camera3D3.");
			_camera3d2.Current = false;
			_camera3d3.Current = true;
			_hasCamera3d3BeenReleased = false;
		}
		// Switch back to Camera3D2 if Camera3D3 is active and RayCast3D2 is released
		else if (_camera3d3.Current && !_rayCast3d2.IsColliding() && !_hasCamera3d3BeenReleased)
		{
			GD.Print("RayCast3D2 is not colliding. Switching back to Camera3D2.");
			_camera3d2.Current = true;
			_camera3d3.Current = false;
			_hasCamera3d3BeenReleased = true;
		}

		// Optional: print what RayCast3D2 is colliding with
		if (_rayCast3d2.IsColliding())
		{
			var collider = _rayCast3d2.GetCollider();
			if (collider is Node node)
			{
				GD.Print($"RayCast3D2 is colliding with node: {node.Name}");
			}
		}
	}
}