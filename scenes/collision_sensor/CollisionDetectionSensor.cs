using Godot;
using System;

public partial class CollisionDetectionSensor : Area3D
{
	[Signal] public delegate void CollisionDetectedEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// init signal connections
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		GD.Print($"CollisionDetectionSensor: OnBodyEntered {body.Name}");
		EmitSignal(nameof(CollisionDetected));
	}
}
