using Godot;
using Godot.Collections;
using System;
//using System.Collections.Generic;

namespace CSE870BPSPrototype
{
    public partial class StateMachine : Node
    {
        [Export]
        public State CurrentState;

        private Dictionary<string, State> States;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            // init private vars
            States = new Dictionary<string, State>();

            // connect child state signals
            foreach (var child in GetChildren())
            {
                if (child is State)
                {
                    States[child.Name] = child as State;
                    (child as State).Transitioned += OnChildTransitioned;
                }
                else
                {
                    GD.PushWarning("State machine contains child which is not 'State'");
                }
            }

            // enter init state
            if (CurrentState != null)
                CurrentState.Enter(new Dictionary());
        }

        // Analogous to _Process()
        public void Update(double delta)
        {
            CurrentState.Update(delta);
        }

        // Analogous to _PhysicsProcess()
        public void PhysicsUpdate(double delta)
        {
            CurrentState.PhysicsUpdate(delta);
        }

        public void TransitionState(string new_state, Dictionary args = null)
        {
            OnChildTransitioned(new_state, args);
        }

        private void OnChildTransitioned(string new_state_name, Dictionary args = null)
        {
            var new_state = States[new_state_name];
            if (new_state != null && new_state != CurrentState)
            {
                GD.Print("StateMachine: OnChildTransitioned!");
                if (CurrentState != null)
                    CurrentState.Exit();
                new_state.Enter(args);
                CurrentState = new_state;
            }
            /*
            else
            {
                GD.PushWarning("Called transition on a state that does not exist");
            }
            */
        }
    }
}