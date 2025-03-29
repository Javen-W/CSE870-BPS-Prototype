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

		private Dictionary<int, string> ScenarioDescriptions = new Dictionary<int, string>()
		{
			{1, "Activation Of System (General Use, no detection and detection):\nDriver shifts the vehicle into reverse. This activates the Pedestrian Backup Assistance System and displays to the driver the rear blindspot. The Pedestrian Backup Assistance System continually monitors the vehicle’s rear trajectory alerting the driver of potential obstacles. During the reverse, an obstacle crosses the rear of the vehicle and is detected by the system.  The system calculates the distance to the obstacle and determines if it needs to give visual and audible warnings to the driver based on the distance. After the driver has finished reversing the vehicle, they will shift the vehicle out of reverse and the system will deactivate."},
			{2, "Vehicle Alerts Driver of Obstacle and Driver Fails to Stop (Automatic Braking):\nDriver shifts the vehicle into reverse. This activates the Pedestrian Backup Assistance System and displays to the driver the rear blindspot. An obstacle crosses begins to cross the rear of the vehicle and a visual and audible warning is given to the driver. The driver is unable to react in time and continues to reverse the vehicle.  Once the vehicle detects that the obstacle is within 1 meter of the rear bumper the system will automatically engage the brakes. The driver can disengage the brakes by holding the brakes and letting go. Once the driver is finished reversing and shifts out of the reverse gear, the system will deactivate."},
			{3, "Vehicle Detects Non Existent Obstacle (Temporarily Mute Alarm):\nDriver is reversing the vehicle and the Pedestrian Backup Assistance System falsely detects there is an obstacle and gives visual and audible warnings to the driver. The driver checks the camera’s feed to see where the obstacle is and sees there is no obstacle. They push a button on the dashboard to mute the warnings and continue reversing. The system does not continue giving warning to the driver until it sees a new obstacle enter the rear blindspot."},
			{4, "Vehicle Collided with Object (Immobilize Vehicle):\nDriver is reversing the vehicle. An obstacle is fast approaching our vehicle and the system displays warnings to the driver. The driver stops their car, but the obstacle continues on its path and collides with the vehicle. Upon the system detecting the obstacle with the collision detector, the system automatically engages the brakes immobilizing the vehicle."},
			{5, "System Detects Sensors are Faulty:\nIf the system detects that less than 2/3 of the sensors are faulty, it notifies the driver that some sensors are malfunctioning but does not shut down. If the system detects that more than 2/3 of the sensors are faulty, it alerts the driver that the system is no longer functional and automatically deactivates."},
			{6, "View Rear Blindspot:\nWhen the system is activated, the system detects objects in the backup camera’s vicinity and creates bounding boxes around the obstacles to identify each object for the driver."},
		};
		
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
			ScenarioDescriptionLabel = GetNode<Label>("Panel/MarginContainer/HBoxContainer/VBoxContainer/HBoxContainer2/ScrollContainer/ScenarioDescriptionLabel");
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
			// Update UI labels
			ScenarioLabel.Text = $"Scenario: ({scenario})";
			ScenarioDescriptionLabel.Text = $"{ScenarioDescriptions[scenario]}";
			
			// Clear existing object panels
			foreach (var objPanel in ObjectPanelContainer.GetChildren())
			{
				objPanel.QueueFree();
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
