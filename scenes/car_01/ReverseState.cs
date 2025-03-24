using Godot;
using Godot.Collections;
using System;

namespace CSE849BPSPrototype
{
    public partial class ReverseState : State
    {
        [Export] public Car01 Car;
		
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
        }
        
        public override void Enter(Dictionary args)
        {
            GD.Print("Entered ReverseState");
            
            Car.VisualDisplayInterfaceSprite.Visible = true;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _PhysicsProcess(double delta)
        {
            if (Input.IsActionPressed("reverse"))
            {
                var speed = Car.LinearVelocity.Length();
                if (speed < 5.0 && speed > 0.01)
                {
                    Car.EngineForce = -Mathf.Clamp(Car.EngineForceValue * Car.BrakeStrength * 5.0f / speed, 0.0f, 100.0f);
                }
                else
                {
                    Car.EngineForce = -Car.EngineForceValue * Car.BrakeStrength;
                }
                
                Car.EngineForce *= Input.GetActionStrength("reverse");
            }

            if (Input.IsActionJustPressed("accelerate"))
            {
                EmitSignal(nameof(Transitioned), "ForwardState", new Dictionary());
            }
        }
    }
}