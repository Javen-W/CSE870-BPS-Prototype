using Godot;
using System;
using System.Collections.Generic;

namespace CSE870BPSPrototype
{
	public partial class Car01 : VehicleBody3D
	{
		[Export] public float SteerSpeed = 1.5f;
		[Export] public float SteerLimit = 0.4f;
		[Export] public float BrakeStrength = 2.0f;
		[Export] public float EngineForceValue = 40.0f;
		[Export] public Node3D CollisionObjects {get; set;}
 		
		public StateMachine StateMachine;
		public SubViewport SubViewportRear;
		public Sprite3D VisualDisplayInterfaceSprite;
		public Camera3D CameraInside;
		public Camera3D CameraSide;
		public Camera3D CameraTop;
		public Camera3D CameraRear;
		public ProximitySensorArray ProximitySensorArray;

		public float SteerTarget;
		public float PreviousSpeed;
		private Dictionary<Node3D, ReferenceRect> _targetRects;
		
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			PreviousSpeed = LinearVelocity.Length();
			_targetRects = new Dictionary<Node3D, ReferenceRect>();
			
			// init reference nodes
			SubViewportRear = GetNode<SubViewport>("Cameras/SubViewportRear");
			VisualDisplayInterfaceSprite = GetNode<Sprite3D>("VisualDisplayInterface/Sprite3D");
			StateMachine = GetNode<StateMachine>("StateMachine");
			CameraInside = GetNode<Camera3D>("Cameras/Camera3DInside");
			CameraSide = GetNode<Camera3D>("Cameras/Camera3DSide");
			CameraTop = GetNode<Camera3D>("Cameras/Camera3DTop");
			CameraRear = GetNode<Camera3D>("Cameras/SubViewportRear/Camera3DRear");
			ProximitySensorArray = GetNode<ProximitySensorArray>("ProximitySensorArray");
			
			// init state machine
			StateMachine.TransitionState("ForwardState", null);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _PhysicsProcess(double delta)
		{
			SteerTarget = Input.GetAxis("turn_right", "turn_left");
			SteerTarget *= SteerLimit;
			
			// process state machine
			StateMachine.PhysicsUpdate(delta);
			
			Steering = (float) Mathf.MoveToward(Steering, SteerTarget, SteerSpeed * delta);
			PreviousSpeed = LinearVelocity.Length();
			UISignalBus.EmitVelocityChanged(PreviousSpeed);
			
			// object highlighting & sensor calculations
			foreach (var obj in CollisionObjects.GetChildren())
			{
				// update reference rectangles
				var objMesh = obj.GetChild(0) as MeshInstance3D;
				UpdateTargetRect(CameraRear, objMesh);
				
				// update UI
				var objBody = obj as StaticBody3D;
				float distance = CameraRear.GlobalTransform.Origin.DistanceTo(objMesh.GlobalTransform.Origin);
				var proximity = ProximitySensorArray.DetectedObjects.Contains(objBody);
				UISignalBus.EmitObjectChangedEvent(objBody, distance, proximity);
			}
		}
		
		private bool IsNodeInCameraFov(Camera3D camera, Node3D target)
		{
			Transform3D cameraTransform = camera.GlobalTransform;
			Vector3 direction = cameraTransform.Origin.DirectionTo(target.GlobalTransform.Origin);
			
			Vector3 cameraForward = -cameraTransform.Basis.Z.Normalized();
			float forwardAlignment = cameraForward.Dot(direction);
			
			float fovAngle = Mathf.Acos(forwardAlignment);
			float fovRadians = Mathf.DegToRad(camera.Fov) / 2.0f;
			
			return fovAngle <= fovRadians && forwardAlignment > 0;
		}
		
		private void UpdateTargetRect(Camera3D camera, MeshInstance3D target)
	    {
			float distance = camera.GlobalTransform.Origin.DistanceTo(target.GlobalTransform.Origin);
			Vector2 screenPos = camera.UnprojectPosition(target.GlobalTransform.Origin);

			// If target isn't in FOV, remove its rect if it exists and return
			if (!IsNodeInCameraFov(camera, target))
			{
			    if (_targetRects.ContainsKey(target))
			    {
			        _targetRects[target].QueueFree();
			        _targetRects.Remove(target);
			    }
			    return;
			}

			// Create or get existing ReferenceRect
			ReferenceRect rect;
			if (!_targetRects.ContainsKey(target))
			{
			    rect = new ReferenceRect();
			    rect.EditorOnly = false;
			    rect.BorderColor = Colors.Green;
			    rect.BorderWidth = 4.0f;
			    
			    SubViewportRear.AddChild(rect);
			    _targetRects[target] = rect;
			}
			else
			{
			    rect = _targetRects[target];
			}

			// Use AABB for better bounds
			Aabb aabb = target.GetAabb();
			Vector3[] corners = GetAabbCorners(aabb, target.GlobalTransform);
			Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);

			foreach (Vector3 corner in corners)
			{
			    Vector2 corner2D = camera.UnprojectPosition(corner);
			    minPos.X = Mathf.Min(minPos.X, corner2D.X);
			    minPos.Y = Mathf.Min(minPos.Y, corner2D.Y);
			    maxPos.X = Mathf.Max(maxPos.X, corner2D.X);
			    maxPos.Y = Mathf.Max(maxPos.Y, corner2D.Y);
			}

			// Calculate original size and position
			Vector2 originalSize = maxPos - minPos;
			Vector2 originalCenter = minPos + (originalSize / 2.0f);

			Vector2 shrunkSize = originalSize * 0.75f;
			rect.Position = originalCenter - (shrunkSize / 2.0f);
			rect.Size = shrunkSize;

			/*
			   // Set position and size (simple fixed-size version)
			   float size = 200.0f;
			   rect.Position = screenPos - new Vector2(size / 2, size / 2);
			   rect.Size = new Vector2(size, size);
			*/

			// Interpolate color: Green (distance >= 10) to Red (distance = 0)
			float t = Mathf.Clamp(distance / 10f, 0.0f, 1.0f);
			Color green = Colors.Green;
			Color red = Colors.Red;
			rect.BorderColor = green.Lerp(red, 1.0f - t + 0.2f);
	    }
		
		private Vector3[] GetAabbCorners(Aabb aabb, Transform3D transform)
		{
			Vector3[] corners = new Vector3[8];
			Vector3 min = aabb.Position;
			Vector3 max = aabb.End;
        
			corners[0] = transform * new Vector3(min.X, min.Y, min.Z);
			corners[1] = transform * new Vector3(max.X, min.Y, min.Z);
			corners[2] = transform * new Vector3(min.X, max.Y, min.Z);
			corners[3] = transform * new Vector3(max.X, max.Y, min.Z);
			corners[4] = transform * new Vector3(min.X, min.Y, max.Z);
			corners[5] = transform * new Vector3(max.X, min.Y, max.Z);
			corners[6] = transform * new Vector3(min.X, max.Y, max.Z);
			corners[7] = transform * new Vector3(max.X, max.Y, max.Z);
        
			return corners;
		}
	}
}
