// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
namespace Com.RonPad.Entities.Fms
{
    /**
	 * This component provider always returns the same instance of the component. The instance
	 * is created when first required and is of the type passed in to the constructor.
	 */
    public class ComponentSingletonProvider : IComponentProvider
    {
        private Type _type;

        /**
		 * Constructor
		 * 
		 * @param type The type of the single instance
		 */
        public ComponentSingletonProvider(Type type)
        {
            _type = type;
        }

        /**
		 * Used to request a component from this provider
		 * 
		 * @return The single instance
		 */
        public object GetComponent()
        {
            return Identifier ??= Activator.CreateInstance(_type);
        }

        /**
		 * Used to compare this provider with others. Any provider that returns the same single
		 * instance will be regarded as equivalent.
		 * 
		 * @return The single instance
		 */
        public object Identifier
        {
	        get;
	        private set;
        }
    }
}
