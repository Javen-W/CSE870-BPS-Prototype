using Godot;
using System;

namespace CSE849BPSPrototype
{
	public partial class ForwardState : State
	{
		[Export] public Car01 Car;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _PhysicsProcess(double delta)
		{
		}
	}
}
