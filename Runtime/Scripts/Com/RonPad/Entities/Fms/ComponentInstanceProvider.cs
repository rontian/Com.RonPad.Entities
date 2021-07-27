// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

namespace Com.RonPad.Entities.Fms
{
    /**
	 * This component provider always returns the same instance of the component. The instance
	 * is passed to the provider at initialisation.
	 */
    public class ComponentInstanceProvider : IComponentProvider
    {

	    /**
		 * Constructor
		 * 
		 * @param instance The instance to return whenever a component is requested.
		 */
        public ComponentInstanceProvider(object instance)
        {
            Identifier = instance;
        }

        /**
		 * Used to request a component from this provider
		 * 
		 * @return The instance
		 */
        public object GetComponent()
        {
            return Identifier;
        }

        /**
		 * Used to compare this provider with others. Any provider that returns the same component
		 * instance will be regarded as equivalent.
		 * 
		 * @return The instance
		 */
        public object Identifier
        {
	        get;
        }
    }
}
