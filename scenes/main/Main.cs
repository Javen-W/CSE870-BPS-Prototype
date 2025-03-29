using Godot;
using System;

namespace CSE870BPSPrototype
{
	public partial class Main : Node3D
	{
		private int currentScenarioIndex = 1;
		private Node3D currentScenario;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Init UI scenario
			UISignalBus.EmitScenarioChanged(currentScenarioIndex);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			var scenarioChanged = HandleScenarioChangeInput();
			if (scenarioChanged)
			{
				UISignalBus.EmitScenarioChanged(currentScenarioIndex);
			}
		}

		private bool HandleScenarioChangeInput()
		{
			if (Input.IsActionJustPressed("scenario1"))
			{
				currentScenarioIndex = 1;
				return true;
			}
			if (Input.IsActionJustPressed("scenario2"))
			{
				currentScenarioIndex = 2;
				return true;
			}
			if (Input.IsActionJustPressed("scenario3"))
			{
				currentScenarioIndex = 3;
				return true;
			}
			if (Input.IsActionJustPressed("scenario4"))
			{
				currentScenarioIndex = 4;
				return true;
			}
			if (Input.IsActionJustPressed("scenario5"))
			{
				currentScenarioIndex = 5;
				return true;
			}
			if (Input.IsActionJustPressed("scenario6"))
			{
				currentScenarioIndex = 6;
				return true;
			}
			return false;
		}
	}
}
