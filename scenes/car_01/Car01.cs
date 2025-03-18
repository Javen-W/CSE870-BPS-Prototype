using Godot;
using System;

namespace CSE849BPSPrototype
{
	public partial class Car01 : VehicleBody3D
	{
		[Export] public float SteerSpeed = 1.5f;
		[Export] public float SteerLimit = 0.4f;
		[Export] public float BrakeStrength = 2.0f;
		[Export] public float EngineForceValue = 40.0f;
		
		public StateMachine StateMachine;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			StateMachine = GetNode<StateMachine>("StateMachine");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
		
		public override void _PhysicsProcess(double delta)
		{
			// process state machine
			StateMachine.PhysicsUpdate(delta);
		}
	}
}
