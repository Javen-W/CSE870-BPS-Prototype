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
            UISignalBus.EmitGearChanged("Reverse");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void PhysicsUpdate(double delta)
        {
            // Emergency braking system
            var emergencyBraking = false;
            if (!Car.AlarmMuted)
            {
                foreach (var obj in Car.CollisionObjects.GetChildren())
                {
                    var objBody = obj as StaticBody3D;
                    var objMesh = obj.GetChild(0) as MeshInstance3D;
                
                    var distance = Car.CalculateObjectDistance(objMesh);
                    var proximity = Car.IsObjectInProximity(objBody, objMesh);

                    if (distance <= Car.AutoBrakingDistance && proximity)
                    {
                        emergencyBraking = true;
                    }
                }
            }
            
            // Acceleration physics
            var isAccelerating = Input.IsActionPressed("accelerate");
            if (isAccelerating)
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
            
            // Braking physics
            var isBraking = emergencyBraking || Input.IsActionPressed("reverse");
            if (isBraking)
            {
                Car.EngineForce = 0.0f;
                Car.Brake = Car.BrakeStrength * 1.5f;
            }
            
            // UI update signals
            UISignalBus.EmitBrakingPressedEvent(isBraking);
            UISignalBus.EmitAcceleratingPressedEvent(isAccelerating);
            
            // State change
            if (Input.IsActionJustPressed("shift_gear"))
            {
                EmitSignal(nameof(Transitioned), "ForwardState", new Dictionary());
            }
        }
    }
}