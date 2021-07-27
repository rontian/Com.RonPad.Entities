// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
using System.Collections.Generic;
using Com.RonPad.Entities.Core;
using Com.RonPad.Entities.Interfaces;
namespace Com.RonPad.Entities.Fms
{
    /**
	 * This is a state machine for the Engine. The state machine manages a set of states,
	 * each of which has a set of System providers. When the state machine changes the state, it removes
	 * Systems associated with the previous state and adds Systems associated with the new state.
	 */
    public class GameEngineStateMachine
    {
        public readonly IGameEngine Engine;
        private Dictionary<string, GameEngineState> _states = new Dictionary<string, GameEngineState>();
        private GameEngineState _currentState;

        /**
		 * Constructor. Creates an SystemStateMachine.
		 */
        public GameEngineStateMachine(IGameEngine engine)
        {
            Engine = engine;
        }

        /**
		 * Add a state to this state machine.
		 *
		 * @param name The name of this state - used to identify it later in the changeState method call.
		 * @param state The state.
		 * @return This state machine, so methods can be chained.
		 */
        public GameEngineStateMachine AddState(string name, GameEngineState state)
        {
            _states.Add(name, state);
            return this;
        }

        /**
		 * Create a new state in this state machine.
		 *
		 * @param name The name of the new state - used to identify it later in the changeState method call.
		 * @return The new EntityState object that is the state. This will need to be configured with
		 * the appropriate component providers.
		 */
        public GameEngineState CreateState(string name)
        {
            var state = new GameEngineState();
            _states.Add(name, state);
            return state;
        }

        /**
		 * Change to a new state. The Systems from the old state will be removed and the Systems
		 * for the new state will be added.
		 *
		 * @param name The name of the state to change to.
		 */
        public void ChangeState(string name)
        {
            if (!_states.ContainsKey(name))
            {
                throw new Exception($"Engine state {name} doesn't exist");
            }
            _states.TryGetValue(name, out var newState);
            if (newState == _currentState)
            {
                newState = null;
                return;
            }
            var toAdd = new Dictionary<object, ISystemProvider>();
            if (newState != null)
            {
                foreach (var provider in newState.Providers)
                {
                    var id = provider.Identifier;
                    toAdd[id] = provider;
                }
                if (_currentState != null)
                {
                    foreach (var provider in _currentState.Providers)
                    {
                        var id = provider.Identifier;
                        if (toAdd.ContainsKey(id))
                        {
                            toAdd.Remove(id);
                        }
                        else
                        {
                            Engine.RemoveSystem(provider.GetSystem());
                        }
                    }
                }
                foreach (var provider in toAdd)
                {
                    Engine.AddSystem(provider.Value.GetSystem(), provider.Value.Priority);
                }
                _currentState = newState;
            }
        }
    }
}
