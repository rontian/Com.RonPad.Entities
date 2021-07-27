// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
using System.Collections.Generic;
namespace Com.RonPad.Entities.Tools
{
    /**
	 * An object pool for re-using components. This is not integrated in to Ash but is used dierectly by 
	 * the developer. It expects components to not require any parameters in their constructor.
	 * 
	 * <p>Fetch an object from the pool with</p>
	 * 
	 * <p>ComponentPool.get( ComponentClass );</p>
	 * 
	 * <p>If the pool contains an object of the required type, it will be returned. If it does not, a new object
	 * will be created and returned.</p>
	 * 
	 * <p>The object returned may have properties set on it from the time it was previously used, so all properties
	 * should be reset in the object once it is received.</p>
	 * 
	 * <p>Add an object to the pool with</p>
	 * 
	 * <p>ComponentPool.dispose( component );</p>
	 * 
	 * <p>You will usually want to do this when removing a component from an entity. The remove method on the entity
	 * returns the component that was removed, so this can be done in one line of code like this</p>
	 * 
	 * <p>ComponentPool.dispose( entity.remove( component ) );</p>
	 */
    public static class ComponentPool
    {
        private static Dictionary<Type, Queue<object>> _pools = new Dictionary<Type, Queue<object>>();

        private static Queue<object> GetPool(Type type)
        {
            if (_pools.TryGetValue(type, out var pool))
            {
                return pool;
            }
            pool = new Queue<object>();
            _pools.Add(type, pool);
            return pool;
        }

        /**
		 * Get an object from the pool.
		 * 
		 * @param componentClass The type of component wanted.
		 * @return The component.
		 */
        public static object GetOne(Type type)
        {
            var pool = GetPool(type);
            return pool.Count > 0 ? pool.Dequeue() : Activator.CreateInstance(type);
        }

        /**
		 * Return an object to the pool for reuse.
		 * 
		 * @param component The component to return to the pool.
		 */
        public static void Dispose(object component)
        {
            if (component != null)
            {
                var type = component.GetType();
                var pool = GetPool(type);
                pool.Enqueue(component);
            }
        }

        /**
		 * Dispose of all pooled resources, freeing them for garbage collection.
		 */
        public static void Clean()
        {
            foreach (var pool in _pools)
            {
                pool.Value.Clear();
            }
            _pools.Clear();
        }
    }
}
