// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
using System.Collections.Generic;
namespace Com.RonPad.Entities.Fms
{
    /**
	 * Represents a state for an EntityStateMachine. The state contains any number of ComponentProviders which
	 * are used to add components to the entity when this state is entered.
	 */
    public class EntityState
    {
        /**
		 * @private
		 */
        internal readonly Dictionary<Type, IComponentProvider> Providers = new Dictionary<Type, IComponentProvider>();

        /**
		 * Add a new ComponentMapping to this state. The mapping is a utility class that is used to
		 * map a component type to the provider that provides the component.
		 * 
		 * @param type The type of component to be mapped
		 * @return The component mapping to use when setting the provider for the component
		 */
        public StateComponentMapping Add(Type type)
        {
            return new StateComponentMapping(this, type);
        }

        /**
		 * Get the ComponentProvider for a particular component type.
		 * 
		 * @param type The type of component to get the provider for
		 * @return The ComponentProvider
		 */
        public IComponentProvider GetProvider(Type type)
        {
            if (Providers.TryGetValue(type, out var provider))
            {
                return provider;
            }
            return null;
        }

        /**
		 * To determine whether this state has a provider for a specific component type.
		 * 
		 * @param type The type of component to look for a provider for
		 * @return true if there is a provider for the given type, false otherwise
		 */
        public bool HasProvider(Type type)
        {
            return Providers.ContainsKey(type);
        }
    }
}
