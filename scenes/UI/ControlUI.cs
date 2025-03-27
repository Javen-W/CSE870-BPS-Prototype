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
			ObjectPanelContainer = GetNode<HBoxContainer>("Panel/MarginContainer/HBoxContainer/ObjectPanelContainer");
			ObjectPanels = new Dictionary<StaticBody3D, ObjectPanel>();
			
			// Init object panels
			foreach (var obj in CollisionObjects.GetChildren())
			{
				var objBody = obj as StaticBody3D;
				var panel = ResourceLoader.Load<PackedScene>("res://scenes/UI/ObjectPanel.tscn").Instantiate() as ObjectPanel;
				ObjectPanels[objBody] = panel;
				ObjectPanelContainer.AddChild(panel);
				panel.NameLabel.Text = objBody.Name;
			}
			
			// Handle signals
			UISignalBus.Instance.GearChanged += OnGearChanged;
			UISignalBus.Instance.VelocityChanged += OnVelocityChanged;
			UISignalBus.Instance.ObjectChanged += OnObjectChanged;
			UISignalBus.Instance.AcceleratingPressed += OnAcceleratingPressed;
			UISignalBus.Instance.BrakingPressed += OnBrakingPressed;
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
	}
}
