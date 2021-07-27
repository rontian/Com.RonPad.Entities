// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
namespace Com.RonPad.Entities.Fms
{
    /**
	 * This component provider calls a function to get the component instance. The function must
	 * return a single component of the appropriate type.
	 */
    public class DynamicComponentProvider : IComponentProvider
    {
        private Func<object> _closure;

        /**
		 * Constructor
		 * 
		 * @param closure The function that will return the component instance when called.
		 */
        public DynamicComponentProvider(Func<object> closure)
        {
            _closure = closure;
        }

        /**
		 * Used to request a component from this provider
		 * 
		 * @return The instance returned by calling the function
		 */
        public object GetComponent()
        {
            return _closure.Invoke();
        }

        /**
		 * Used to compare this provider with others. Any provider that uses the function or method 
		 * closure to provide the instance is regarded as equivalent.
		 * 
		 * @return The function
		 */
        public object Identifier => _closure;
    }
}
