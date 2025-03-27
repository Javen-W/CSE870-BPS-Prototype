using Godot;
using Godot.Collections;
using System;

namespace CSE870BPSPrototype
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
        public override void PhysicsUpdate(double delta)
        {
            // object distance calculations
            foreach (var obj in Car.CollisionObjects.GetChildren())
            {
                var objBody = obj as StaticBody3D;
                var isDetected = Car.ProximitySensorArray.DetectedObjects.Contains(objBody);
                if (isDetected)
                {
                    var objMesh = obj.GetChild(0) as MeshInstance3D;
                    float distance = Car.CameraRear.GlobalTransform.Origin.DistanceTo(objMesh.GlobalTransform.Origin);
                    GD.Print("Distance: " + distance);
                }
            }
            
            if (Input.IsActionPressed("accelerate"))
            {
                var speed = Car.LinearVelocity.Length();
                if (speed < 2.5 && speed > 0.01)
                {
                    Car.EngineForce = -Mathf.Clamp(Car.EngineForceValue * Car.BrakeStrength * 2.5f / speed, 0.0f, 100.0f);
                }
                else
                {
                    Car.EngineForce = -Car.EngineForceValue * Car.BrakeStrength * 0.5f;
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
                EmitSignal(nameof(Transitioned), "ForwardState", new Dictionary());
            }
        }
    }
}