// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
using Com.RonPad.Entities.Core;
namespace Com.RonPad.Entities.Fms
{
    /**
     * This System provider returns results of a method call. The method
     * is passed to the provider at initialisation.
     */
    public class DynamicSystemProvider : ISystemProvider
    {
        private Func<SystemBase> _method;
        /**
         * Constructor
         *
         * @param method The method that returns the System instance;
         */
        public DynamicSystemProvider(Func<SystemBase> method)
        {
            _method = method;
        }

        /**
         * Used to request a component from this provider
         *
         * @return The instance of the System
         */
        public SystemBase GetSystem()
        {
            return _method.Invoke();
        }

        /**
         * Used to compare this provider with others. Any provider that returns the same component
         * instance will be regarded as equivalent.
         *
         * @return The method used to call the System instances
         */
        public object Identifier => _method;

        /**
         * The priority at which the System should be added to the Engine
         */
        public int Priority { get; set; }
    }
}
