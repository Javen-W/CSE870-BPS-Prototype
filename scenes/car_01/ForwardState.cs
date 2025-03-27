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

			Car.VisualDisplayInterfaceSprite.Visible = false;
			UISignalBus.EmitGearChanged("Forward");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void PhysicsUpdate(double delta)
		{
			if (Input.IsActionPressed("accelerate"))
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
			}
			
			if (Input.IsActionPressed("reverse"))
			{
				Car.EngineForce = 0.0f;
				Car.Brake = 15f;
			}
			
			if (Input.IsActionJustPressed("shift_gear"))
			{
				EmitSignal(nameof(Transitioned), "ReverseState", new Dictionary());
			}
		}
	}
}
