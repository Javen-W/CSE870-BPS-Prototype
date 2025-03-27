using Godot;
using System;

namespace CSE870BPSPrototype
{
	public partial class ObjectPanel : Control
	{
		public Label NameLabel { get; set; }
		public Label DistanceLabel { get; set; }
		public Label ProximityLabel { get; set; }
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Init node references
			NameLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/NameLabel");
			DistanceLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/DistanceLabel");
			ProximityLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/ProximityLabel");
		}
	}
}