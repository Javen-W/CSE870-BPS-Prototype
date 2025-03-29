using Godot;
using System;
using System.Collections.Generic;

namespace CSE870BPSPrototype
{
	public partial class ControlUI : Control
	{
		[Export] public Node3D CollisionObjects {get; set;}

		private Label GearLabel;
		private Label VelocityLabel;
		private Label AcceleratingLabel;
		private Label BrakingLabel;
		private Label CameraLabel;
		private Label ScenarioLabel;
		private Label ScenarioDescriptionLabel;
		private Label MutedLabel;
		private Label CollidedLabel;
		
		private HBoxContainer ObjectPanelContainer;
		private Dictionary<StaticBody3D, ObjectPanel> ObjectPanels;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Init node references
			GearLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer/GearLabel");
			VelocityLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer/VelocityLabel");
			AcceleratingLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer2/AcceleratingLabel");
			BrakingLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer2/BrakingLabel");
			CameraLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer3/CameraLabel");
			ScenarioLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer3/ScenarioLabel");
			ScenarioDescriptionLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer2/ScenarioDescriptionLabel");
			MutedLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer4/MutedLabel");
			CollidedLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer/VBoxContainer4/CollidedLabel");
			
			ObjectPanelContainer = GetNode<HBoxContainer>("Panel/MarginContainer/HBoxContainer/ObjectPanelContainer");
			ObjectPanels = new Dictionary<StaticBody3D, ObjectPanel>();
			
			// Handle signals
			UISignalBus.Instance.GearChanged += OnGearChanged;
			UISignalBus.Instance.VelocityChanged += OnVelocityChanged;
			UISignalBus.Instance.ObjectChanged += OnObjectChanged;
			UISignalBus.Instance.AcceleratingPressed += OnAcceleratingPressed;
			UISignalBus.Instance.BrakingPressed += OnBrakingPressed;
			UISignalBus.Instance.CameraCycled += OnCameraCycled;
			UISignalBus.Instance.ScenarioChanged += OnScenarioChanged;
			UISignalBus.Instance.AlarmMuted += OnAlarmMuted;
			UISignalBus.Instance.CollisionDetected += OnCollisionDetected;
		}
		
		private void OnScenarioChanged(int scenario, Node3D collisionObjects)
		{
			ScenarioLabel.Text = $"Scenario: {scenario}";
			ScenarioDescriptionLabel.Text = $"Scenario({scenario}): ...";
			
			// Clear existing object panels
			foreach (var objBody in ObjectPanels.Keys)
			{
				ObjectPanels[objBody].QueueFree();
			}
			
			// Init object panels
			CollisionObjects = collisionObjects;
			foreach (var obj in CollisionObjects.GetChildren())
			{
				var objBody = obj as StaticBody3D;
				var panel = ResourceLoader.Load<PackedScene>("res://scenes/UI/ObjectPanel.tscn").Instantiate() as ObjectPanel;
				ObjectPanels[objBody] = panel;
				ObjectPanelContainer.AddChild(panel);
				panel.NameLabel.Text = objBody.Name;
			}
		}

		private void OnCollisionDetected(bool collisionDetected)
		{
			CollidedLabel.Text = $"Collided: {collisionDetected}";
		}

		private void OnAlarmMuted(bool muted)
		{
			MutedLabel.Text = $"Muted: {muted}";
		}

		private void OnGearChanged(string gear)
		{
			GearLabel.Text = $"Gear: {gear}";
		}

		private void OnVelocityChanged(float velocity)
		{
			VelocityLabel.Text = $"Velocity: {velocity:F3}";
		}

		private void OnObjectChanged(StaticBody3D obj, float distance, bool proximity)
		{
			ObjectPanels[obj].DistanceLabel.Text = $"Distance: {distance:F3}";
			ObjectPanels[obj].ProximityLabel.Text = $"Proximity: {proximity}";
		}

		private void OnAcceleratingPressed(bool accelerating)
		{
			AcceleratingLabel.Text = $"Accelerating: {accelerating}";
		}

		private void OnBrakingPressed(bool braking)
		{
			BrakingLabel.Text = $"Braking: {braking}";
		}

		private void OnCameraCycled(string camera)
		{
			CameraLabel.Text = $"Camera: {camera}";
		}
	}
}
