// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
namespace Com.RonPad.Entities.Fms
{
    public class SystemSingletonProvider : ISystemProvider
    {
        private Type _type;
        private Core.System _instance;

        /**
         * Constructor
         *
         * @param type The type of the single System instance
         */
        public SystemSingletonProvider(Type type)
        {
            _type = type;
        }

        /**
         * Used to request a System from this provider
         *
         * @return The single instance
         */
        public Core.System GetSystem()
        {
            return _instance ??= (Core.System)Activator.CreateInstance(_type);
        }

        /**
         * Used to compare this provider with others. Any provider that returns the same single
         * instance will be regarded as equivalent.
         *
         * @return The single instance
         */
        public object Identifier => _instance;

        /**
         * The priority at which the System should be added to the Engine
         */
        public int Priority { get; set; }
    }
}
