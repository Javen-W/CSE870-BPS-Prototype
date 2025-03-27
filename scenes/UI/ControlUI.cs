using Godot;
using System;

namespace CSE870BPSPrototype
{
	public partial class ControlUI : Control
	{
		private Label GearLabel;
		private Label VelocityLabel;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Init node references
			GearLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/HBoxContainer/VBoxContainer/GearLabel");
			VelocityLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/HBoxContainer/VBoxContainer/VelocityLabel");
			
			// Handle signals
			UISignalBus.Instance.GearChanged += OnGearChanged;
			UISignalBus.Instance.VelocityChanged += OnVelocityChanged;
		}

		private void OnGearChanged(string gear)
		{
			GearLabel.Text = $"Gear: {gear}";
		}

		private void OnVelocityChanged(float velocity)
		{
			VelocityLabel.Text = $"Velocity: {velocity:F3}";
		}
	}
}
