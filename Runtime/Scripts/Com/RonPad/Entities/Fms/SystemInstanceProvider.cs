// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

namespace Com.RonPad.Entities.Fms
{
    public class SystemInstanceProvider : ISystemProvider
    {
        private Core.System _instance;

        /**
         * Constructor
         *
         * @param instance The instance to return whenever a System is requested.
         */
        public SystemInstanceProvider(Core.System instance)
        {
            _instance = instance;
        }

        /**
         * Used to request a component from this provider
         *
         * @return The instance of the System
         */
        public Core.System GetSystem()
        {
            return _instance;
        }

        /**
         * Used to compare this provider with others. Any provider that returns the same component
         * instance will be regarded as equivalent.
         *
         * @return The instance
         */
        public object Identifier => _instance;

        /**
         * The priority at which the System should be added to the Engine
         */
        public int Priority { get; set; }

    }
}
