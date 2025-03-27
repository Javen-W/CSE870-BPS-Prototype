using Godot;
using System;

namespace CSE870BPSPrototype
{
	public partial class ControlUI : Control
	{
		private Label GearLabel;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Init node references
			GearLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/HBoxContainer/VBoxContainer/GearLabel");
			
			// Handle signals
			UISignalBus.Instance.GearChanged += OnGearChanged;
		}

		private void OnGearChanged(string gear)
		{
			GearLabel.Text = $"Gear: {gear}";
		}
	}
}
