using Godot;
using System;
using System.Collections.Generic;

namespace CSE870BPSPrototype
{
	public partial class ProximitySensorArray : Area3D
	{
		public List<StaticBody3D> DetectedObjects;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			GD.Print("ProximitySensorArray Ready()");
			DetectedObjects = new List<StaticBody3D>();
		
			// init signal connections
			BodyEntered += OnBodyEntered;
			BodyExited += OnBodyExited;
		}

		private void OnBodyEntered(Node body)
		{
			DetectedObjects.Add(body as StaticBody3D);
			GD.Print($"ProximitySensorArray: detected_objs={DetectedObjects.Count}");
		}

		private void OnBodyExited(Node body)
		{
			DetectedObjects.Remove(body as StaticBody3D);
			GD.Print($"ProximitySensorArray: detected_objs={DetectedObjects.Count}");
		}
	}
}

