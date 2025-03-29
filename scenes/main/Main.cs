using Godot;
using System;

namespace CSE870BPSPrototype
{
	public partial class Main : Node3D
	{
		private int currentScenarioIndex;
		private Node3D currentScenario;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Init UI scenario
			currentScenarioIndex = 1;
			currentScenario = GetNode<Node3D>("Scenario1");
			var collisionObjects = currentScenario.GetChild(2) as Node3D;
			UISignalBus.EmitScenarioChanged(currentScenarioIndex, collisionObjects);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			HandleScenarioChangeInput();
		}

		private void ChangeScenarioScene(int scenarioIndex, string scenarioPath)
		{
			currentScenarioIndex = scenarioIndex;
			if (currentScenario != null)
			{
				currentScenario.QueueFree();
			}
			currentScenario = ResourceLoader.Load<PackedScene>(scenarioPath).Instantiate<Node3D>();
			AddChild(currentScenario);
			var collisionObjects = currentScenario.GetChild(2) as Node3D;
			UISignalBus.EmitScenarioChanged(currentScenarioIndex, collisionObjects);
		}

		private void HandleScenarioChangeInput()
		{
			if (Input.IsActionJustPressed("scenario1"))
			{
				ChangeScenarioScene(1, "res://scenes/scenarios/Scenario1.tscn");
			}
			else if (Input.IsActionJustPressed("scenario2"))
			{
				ChangeScenarioScene(2, "res://scenes/scenarios/Scenario2.tscn");
			}
			else if (Input.IsActionJustPressed("scenario3"))
			{
				ChangeScenarioScene(3, "res://scenes/scenarios/Scenario3.tscn");
			}
			else if (Input.IsActionJustPressed("scenario4"))
			{
				ChangeScenarioScene(4, "res://scenes/scenarios/Scenario4.tscn");
			}
			else if (Input.IsActionJustPressed("scenario5"))
			{
				ChangeScenarioScene(5, "res://scenes/scenarios/Scenario5.tscn");
			}
			else if (Input.IsActionJustPressed("scenario6"))
			{
				ChangeScenarioScene(6, "res://scenes/scenarios/Scenario6.tscn");
			}
		}
	}
}
