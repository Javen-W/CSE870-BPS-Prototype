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
		public SubViewport SubViewportRear;
		public Sprite3D VisualDisplayInterfaceSprite;

		public float SteerTarget;
		public float PreviousSpeed;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			PreviousSpeed = LinearVelocity.Length();
			
			SubViewportRear = GetNode<SubViewport>("Cameras/SubViewportRear");
			VisualDisplayInterfaceSprite = GetNode<Sprite3D>("VisualDisplayInterface/Sprite3D");
			StateMachine = GetNode<StateMachine>("StateMachine");
			
			StateMachine.TransitionState("ForwardState", null);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _PhysicsProcess(double delta)
		{
			SteerTarget = Input.GetAxis("turn_right", "turn_left");
			SteerTarget *= SteerLimit;
			
			// process state machine
			StateMachine.PhysicsUpdate(delta);
			
			Steering = (float) Mathf.MoveToward(Steering, SteerTarget, SteerSpeed * delta);
			PreviousSpeed = LinearVelocity.Length();
		}
	}
}
