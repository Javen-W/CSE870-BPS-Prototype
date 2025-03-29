using Godot;
using Godot.Collections;
using System;

namespace CSE870BPSPrototype
{
    public partial class CollisionState : State
    {
        [Export] public Car01 Car;
		
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
        }
        
        public override void Enter(Dictionary args)
        {
            GD.Print("Entered CollisionState");
            UISignalBus.EmitCollisionDetected(true);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void PhysicsUpdate(double delta)
        {
            // Braking physics
            Car.EngineForce = 0.0f;
            Car.Brake = Car.BrakeStrength * 1.5f;
            
            // UI update signals
            UISignalBus.EmitBrakingPressedEvent(true);
            UISignalBus.EmitAcceleratingPressedEvent(false);
        }
    }
}