// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
using System.Collections.Generic;
using Com.RonPad.Entities.Core;
namespace Com.RonPad.Entities.Fms
{
    /**
	 * This is a state machine for an entity. The state machine manages a set of states,
	 * each of which has a set of component providers. When the state machine changes the state, it removes
	 * components associated with the previous state and adds components associated with the new state.
	 */
    public class EntityStateMachine
    {
        private Dictionary<string, EntityState> _states = new Dictionary<string, EntityState>();
        /**
         * The current state of the state machine.
         */
        private EntityState _currentState;
        /**
         * The entity whose state machine this is
         */
        public readonly Entity Entity;

        /**
         * Constructor. Creates an EntityStateMachine.
         */
        public EntityStateMachine(Entity entity)
        {
            Entity = entity;
        }

        /**
         * Add a state to this state machine.
         * 
         * @param name The name of this state - used to identify it later in the changeState method call.
         * @param state The state.
         * @return This state machine, so methods can be chained.
         */
        public EntityStateMachine AddState(string name, EntityState state)
        {
            _states[name] = state;
            return this;
        }

        /**
         * Create a new state in this state machine.
         * 
         * @param name The name of the new state - used to identify it later in the changeState method call.
         * @return The new EntityState object that is the state. This will need to be configured with
         * the appropriate component providers.
         */
        public EntityState CreateState(string name)
        {
            var state = new EntityState();
            _states[name] = state;
            return state;
        }

        /**
         * Change to a new state. The components from the old state will be removed and the components
         * for the new state will be added.
         * 
         * @param name The name of the state to change to.
         */
        public void ChangeState(string name)
        {
            var newState = _states[name];
            if (newState == null)
            {
                throw new Exception($"Entity state {name} doesn't exist");
            }
            if (newState == _currentState)
            {
                newState = null;
                return;
            }
            Type type;
            var toAdd = new Dictionary<Type, IComponentProvider>();
            if (_currentState != null)
            {
                foreach (var t in newState.Providers)
                {
                    type = t.Key;
                    toAdd[type] = newState.Providers[type];
                }
                foreach (var t in _currentState.Providers)
                {
                    type = t.Key;
                    if (toAdd.TryGetValue(type, out var other) && other.Identifier == _currentState.Providers[type].Identifier)
                    {
                        toAdd.Remove(type);
                    }
                    else
                    {
                        Entity.Remove(type);
                    }
                }
            }
            else
            {
                toAdd = newState.Providers;
            }
            foreach (var t in toAdd)
            {
                type = t.Key;
                Entity.Add(toAdd[type].GetComponent(), type);
            }
            _currentState = newState;
        }
    }
}
