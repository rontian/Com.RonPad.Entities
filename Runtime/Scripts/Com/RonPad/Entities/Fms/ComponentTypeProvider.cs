// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
namespace Com.RonPad.Entities.Fms
{
    /**
	 * This component provider always returns a new instance of a component. An instance
	 * is created when requested and is of the type passed in to the constructor.
	 */
    public class ComponentTypeProvider : IComponentProvider
    {
        private Type _type;

        /**
		 * Constructor
		 * 
		 * @param type The type of the instances to be created
		 */
        public ComponentTypeProvider(Type type)
        {
            _type = type;
        }

        /**
		 * Used to request a component from this provider
		 * 
		 * @return A new instance of the type provided in the constructor
		 */
        public object GetComponent()
        {
            return Activator.CreateInstance(_type);
        }

        /**
		 * Used to compare this provider with others. Any ComponentTypeProvider that returns
		 * the same type will be regarded as equivalent.
		 * 
		 * @return The type of the instances created
		 */
        public object Identifier => _type;
    }
}
