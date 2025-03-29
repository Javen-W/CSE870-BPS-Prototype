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
            Car.AlarmSFXPlayer.Playing = true;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void PhysicsUpdate(double delta)
        {
            // Emergency braking & alarm systems
            var emergencyBraking = false;
            if (!Car.AlarmMuted)
            {
                var minDist = float.MaxValue;
                foreach (var obj in Car.CollisionObjects.GetChildren())
                {
                    var objBody = obj as StaticBody3D;
                    var objMesh = obj.GetChild(0) as MeshInstance3D;
                
                    var distance = Car.CalculateObjectDistance(objMesh);
                    var proximity = Car.IsObjectInProximity(objBody, objMesh);
                    
                    // Find nearest object
                    if (distance <= Car.ObjectAlarmDistance)  // && proximity
                    {
                        minDist = Math.Min(minDist, distance);
                    }
                    
                    // Enable emergency braking?
                    if (distance <= Car.AutoBrakingDistance && proximity)
                    {
                        emergencyBraking = true;
                    }
                }
                
                // Calculate alarm frequency
                if (minDist <= Car.ObjectAlarmDistance)
                {
                    Car.AlarmSFXPlayer.StreamPaused = false;
                    Car.AlarmSFXPlayer.PitchScale = Mathf.Lerp(0.8f, 0.5f, minDist / Car.ObjectAlarmDistance);
                }
                else
                {
                    Car.AlarmSFXPlayer.StreamPaused = true;
                }
                
            }
            else
            {
                Car.AlarmSFXPlayer.StreamPaused = true;
            }
            
            // Acceleration physics
            var isAccelerating = Input.IsActionPressed("accelerate");
            if (isAccelerating)
            {
                var speed = Car.LinearVelocity.Length();
                if (speed < 2.5 && speed > 0.01)
                {
                    Car.EngineForce = -Mathf.Clamp(Car.EngineForceValue * Car.BrakeStrength / speed, 0.0f, 100.0f);
                }
                else
                {
                    Car.EngineForce = -Car.EngineForceValue * 0.5f;
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
                Car.Brake = Car.BrakeStrength * 2.5f;
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