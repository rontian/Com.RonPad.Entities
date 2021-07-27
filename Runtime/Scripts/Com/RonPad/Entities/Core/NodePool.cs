// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/20
// *************************************************/

using System;
using System.Collections.Generic;
namespace Com.RonPad.Entities.Core
{
    /**
	 * This internal class maintains a pool of deleted nodes for reuse by the framework. This reduces the overhead
	 * from object creation and garbage collection.
	 * 
	 * Because nodes may be deleted from a NodeList while in use, by deleting Nodes from a NodeList
	 * while iterating through the NodeList, the pool also maintains a cache of nodes that are added to the pool
	 * but should not be reused yet. They are then released into the pool by calling the releaseCache method.
	 */
    public class NodePool<T> where T : Node
    {
        private T _tail;
        private Type _type;
        private T _cacheTail;
        private Dictionary<Type, string> _components;

        /**
		 * Creates a pool for the given node class.
		 */
        public NodePool(Dictionary<Type, string> components)
        {
            _type = typeof(T);
            _components = components;
        }

        /**
		 * Fetches a node from the pool.
		 */
        internal T GetOne()
        {
            if (_tail != null)
            {
                var node = _tail;
                _tail = (T)_tail.Previous;
                node.Previous = null;
                return node;
            }
            else
            {
                return (T)Activator.CreateInstance(_type);
            }
        }

        /**
		 * Adds a node to the pool.
		 */
        internal void Dispose(T node)
        {
            foreach (var item in _components)
            {
                var type = node.GetType();
                var properties = type.GetProperties();
                foreach (var info in properties)
                {
                    info.SetValue(node, null);
                }
            }
            node.Entity = null;

            node.Next = null;
            node.Previous = _tail;
            _tail = node;
        }

        /**
		 * Adds a node to the cache
		 */
        internal void Cache(T node)
        {
            node.Previous = _cacheTail;
            _cacheTail = node;
        }

        /**
		 * Releases all nodes from the cache into the pool
		 */
        internal void ReleaseCache()
        {
            while (_cacheTail != null)
            {
                var node = _cacheTail;
                _cacheTail = (T)node.Previous;
                Dispose(node);
            }
        }
    }
}
