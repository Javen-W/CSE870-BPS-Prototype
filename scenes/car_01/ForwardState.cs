using Godot;
using Godot.Collections;
using System;

namespace CSE870BPSPrototype
{
	public partial class ForwardState : State
	{
		[Export] public Car01 Car;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		public override void Enter(Dictionary args)
		{
			GD.Print("Entered ForwardState");
			UISignalBus.EmitGearChanged("Forward");
			if (!Car.AlwaysDisplayBackupCamera)
			{
				Car.VisualDisplayInterfaceSprite.Visible = false;
			}
			Car.AlarmSFXPlayer.Playing = false;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void PhysicsUpdate(double delta)
		{
			// Acceleration physics
			var isAccelerating = Input.IsActionPressed("accelerate");
			if (isAccelerating)
			{
				var speed = Car.LinearVelocity.Length();
				if (speed < 5.0 && speed > 0.01)
				{
					Car.EngineForce = Mathf.Clamp(Car.EngineForceValue * 5.0f / speed, 0.0f, 100.0f);
				}
				else
				{
					Car.EngineForce = Car.EngineForceValue;
				}
				
				Car.EngineForce *= Input.GetActionStrength("accelerate");
			}
			else
			{
				Car.EngineForce = 0.0f;
				Car.Brake = Car.BrakeStrength * 0.5f;
			}
			
			// Braking physics
			var isBraking = Input.IsActionPressed("reverse");
			if (isBraking)
			{
				Car.EngineForce = 0.0f;
				Car.Brake = Car.BrakeStrength;
			}
			
			// UI update signals
			UISignalBus.EmitBrakingPressedEvent(isBraking);
			UISignalBus.EmitAcceleratingPressedEvent(isAccelerating);
			
			// State change
			if (Input.IsActionJustPressed("shift_gear"))
			{
				EmitSignal(nameof(Transitioned), "ReverseState", new Dictionary());
			}
		}
	}
}
