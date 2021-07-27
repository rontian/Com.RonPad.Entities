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
	 * Represents a state for a SystemStateMachine. The state contains any number of SystemProviders which
	 * are used to add Systems to the World when this state is entered.
	 */
    public class GameState
    {
        internal readonly List<ISystemProvider> Providers = new List<ISystemProvider>();

        /**
         * Creates a mapping for the System type to a specific System instance. A
         * SystemInstanceProvider is used for the mapping.
         *
         * @param system The System instance to use for the mapping
         * @return This StateSystemMapping, so more modifications can be applied
         */
        public StateSystemMapping AddInstance(Core.System system)
        {
            return AddProvider(new SystemInstanceProvider(system));
        }

        /**
         * Creates a mapping for the System type to a single instance of the provided type.
         * The instance is not created until it is first requested. The type should be the same
         * as or extend the type for this mapping. A SystemSingletonProvider is used for
         * the mapping.
         *
         * @param type The type of the single instance to be created. If omitted, the type of the
         * mapping is used.
         * @return This StateSystemMapping, so more modifications can be applied
         */
        public StateSystemMapping AddSingleton(Type type)
        {
            return AddProvider(new SystemSingletonProvider(type));

        }

        /**
         * Creates a mapping for the System type to a method call.
         * The method should return a System instance. A DynamicSystemProvider is used for
         * the mapping.
         *
         * @param method The method to provide the System instance.
         * @return This StateSystemMapping, so more modifications can be applied.
         */
        public StateSystemMapping AddMethod(Func<Core.System> method)
        {
            return AddProvider(new DynamicSystemProvider(method));
        }

        /**
         * Adds any SystemProvider.
         *
         * @param provider The component provider to use.
         * @return This StateSystemMapping, so more modifications can be applied.
         */
        public StateSystemMapping AddProvider(ISystemProvider provider)
        {
            var mapping = new StateSystemMapping(this, provider);
            Providers.Add(provider);
            return mapping;
        }
    }
}
