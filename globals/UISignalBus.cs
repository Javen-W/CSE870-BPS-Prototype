using Godot;
using System;

namespace CSE870BPSPrototype
{
	public partial class UISignalBus : Node
	{
		[Signal] public delegate void GearChangedEventHandler(string gear);
		
		[Signal] public delegate void VelocityChangedEventHandler(float velocity);
		
		[Signal] public delegate void ObjectChangedEventHandler(StaticBody3D obj, float distance, bool proximity);
		
		[Signal] public delegate void AcceleratingPressedEventHandler(bool accelerating);
		
		[Signal] public delegate void BrakingPressedEventHandler(bool braking);
		
		[Signal] public delegate void CameraCycledEventHandler(string camera);
		
		[Signal] public delegate void ScenarioChangedEventHandler(int scenario, Node3D collisionObjects);
		
		[Signal] public delegate void AlarmMutedEventHandler(bool alarmMuted);
		
		[Signal] public delegate void CollisionDetectedEventHandler(bool collisionDetected);

		public static void EmitScenarioChanged(int scenario, Node3D collisionObjects)
		{
			Instance.EmitSignal(nameof(ScenarioChanged), scenario, collisionObjects);
		}

		public static void EmitAlarmMuted(bool alarmMuted)
		{
			Instance.EmitSignal(nameof(AlarmMuted), alarmMuted);
		}

		public static void EmitCollisionDetected(bool collisionDetected)
		{
			Instance.EmitSignal(nameof(CollisionDetected), collisionDetected);
		}
	
		public static void EmitGearChanged(string gear)
		{
			Instance.EmitSignal(nameof(GearChanged), gear);
		}

		public static void EmitVelocityChanged(float velocity)
		{
			Instance.EmitSignal(nameof(VelocityChanged), velocity);
		}

		public static void EmitObjectChangedEvent(StaticBody3D obj, float distance, bool proximity)
		{
			Instance.EmitSignal(nameof(ObjectChanged), obj, distance, proximity);
		}

		public static void EmitAcceleratingPressedEvent(bool accelerating)
		{
			Instance.EmitSignal(nameof(AcceleratingPressed), accelerating);
		}

		public static void EmitBrakingPressedEvent(bool braking)
		{
			Instance.EmitSignal(nameof(BrakingPressed), braking);
		}
		
		public static void EmitCameraCycled(string camera)
		{
			Instance.EmitSignal(nameof(CameraCycled), camera);
		}
		
		public static UISignalBus Instance { get; set; }
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Instance = this;
		}
	}
}

