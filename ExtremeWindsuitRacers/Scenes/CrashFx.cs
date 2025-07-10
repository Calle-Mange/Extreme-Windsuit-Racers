using Godot;
using System;

public partial class CrashFx : Node3D
{
	private Node3D _glider;
	private Transform3D _gliderStartPosition;
	private AudioStreamPlayer3D _audioPlayer;
	private Decal _bloodDecal;
	private Area3D _gliderHitBox;

	public override void _Ready()
	{
		_audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
		_glider = GetParent<CharacterBody3D>();
		_gliderStartPosition = _glider.GlobalTransform;

		// Get the GliderHitbox node (child of Glider)
		_gliderHitBox = _glider.GetNode<Area3D>("GliderHitbox");

		// Connect the BodyEntered signal to a handler
		_gliderHitBox.BodyEntered += OnGliderHitboxCollision;

		_bloodDecal = GetNode<Decal>("BloodDecal");
		_bloodDecal.Visible = false; // Hide blood decal at start
	}

	private void OnGliderHitboxCollision(Node body)
	{
		var gliderParent = _glider.GetParent<Node3D>();
		if (gliderParent != null && _bloodDecal != null)
		{
			// Duplicate the blood decal (deep copy)
			var newDecal = (Decal)_bloodDecal.Duplicate();

			// Set its transform to the parent's position (adjust as needed)
			newDecal.GlobalTransform = new Transform3D(
				_bloodDecal.GlobalTransform.Basis,
				_glider.GlobalTransform.Origin
			);

			// Make sure the new decal is visible
			newDecal.Visible = true;

			// Add the new decal to the parent
			gliderParent.AddChild(newDecal);
		}

		_audioPlayer?.Play();
	}
}