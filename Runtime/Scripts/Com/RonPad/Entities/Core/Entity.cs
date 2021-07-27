// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/20
// *************************************************/

using System;
using System.Collections.Generic;
using Com.RonPad.Signals;
namespace Com.RonPad.Entities.Core
{
    /**
	 * An entity is composed from components. As such, it is essentially a collection object for components. 
	 * Sometimes, the entities in a game will mirror the actual characters and objects in the game, but this 
	 * is not necessary.
	 * 
	 * <p>Components are simple value objects that contain data relevant to the entity. Entities
	 * with similar functionality will have instances of the same components. So we might have 
	 * a position component</p>
	 * 
	 * <p><code>public class PositionComponent
	 * {
	 *   public var x : Number;
	 *   public var y : Number;
	 * }</code></p>
	 * 
	 * <p>All entities that have a position in the game world, will have an instance of the
	 * position component. Systems operate on entities based on the components they have.</p>
	 */
    public class Entity
    {
        internal static int NameCount = 0;
        /**
		 * Optional, give the entity a name. This can help with debugging and with serialising the entity.
		 */
        public string Name { get; }
        /**
		 * This signal is dispatched when a component is added to the entity.
		 */
        public readonly Signal2<Entity, Type> ComponentAdded = new Signal2<Entity, Type>();
        /**
		 * This signal is dispatched when a component is removed from the entity.
		 */
        public readonly Signal2<Entity, Type> ComponentRemoved = new Signal2<Entity, Type>();
        /**
		 * Dispatched when the name of the entity changes. Used internally by the engine to track entities based on their names.
		 */
        internal readonly Signal2<Entity, string> NameChanged = new Signal2<Entity, string>();

        internal Entity Previous;
        internal Entity Next;
        internal readonly Dictionary<Type, object> Components = new Dictionary<Type, object>();
        public Entity(string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                Name = "_entity" + (++NameCount);
            }
            else
            {
                Name = name;
            }
        }
        /**
		 * Add a component to the entity.
		 * 
		 * @param component The component object to add.
		 * @param componentClass The class of the component. This is only necessary if the component
		 * extends another component class and you want the framework to treat the component as of 
		 * the base class type. If not set, the class type is determined directly from the component.
		 * 
		 * @return A reference to the entity. This enables the chaining of calls to add, to make
		 * creating and configuring entities cleaner. e.g.
		 * 
		 * <code>var entity : Entity = new Entity()
		 *     .add( new Position( 100, 200 )
		 *     .add( new Display( new PlayerClip() );</code>
		 */
        public Entity Add(object component, Type type = null)
        {
            type ??= component.GetType();
            if (Components.ContainsKey(type))
            {
                Remove(type);
            }
            Components.Add(type, component);
            ComponentAdded.Dispatch(this, type);
            return this;
        }

        /**
		 * Remove a component from the entity.
		 * 
		 * @param componentClass The class of the component to be removed.
		 * @return the component, or null if the component doesn't exist in the entity
		 */
        public T Remove<T>()
        {
            return (T)Remove(typeof(T));
        }
        public object Remove(Type type)
        {
            if (Components.TryGetValue(type, out var component))
            {
                Components.Remove(type);
                ComponentRemoved.Dispatch(this, type);
                return component;
            }
            return null;
        }

        /**
		 * Get a component from the entity.
		 * 
		 * @param componentClass The class of the component requested.
		 * @return The component, or null if none was found.
		 */
        public T GetComponent<T>()
        {
            return (T)GetComponent(typeof(T));
        }
        public object GetComponent(Type type)
        {
            return Components[type];
        }

        /**
		 * Get all components from the entity.
		 * 
		 * @return An array containing all the components that are on the entity.
		 */
        public object[] GetComponents()
        {
            var index = 0;
            var components = new object[Components.Count];
            foreach (var item in Components)
            {
                components[index] = item.Value;
                index++;
            }
            return components;
        }
        public void GetComponents(List<object> components)
        {
            if (components == null)
            {
                throw new Exception("results is invalid.");
            }
            components.Clear();
            foreach (var item in Components)
            {
                components.Add(item.Value);
            }
        }

        /**
		 * Does the entity have a component of a particular type.
		 * 
		 * @param componentClass The class of the component sought.
		 * @return true if the entity has a component of the type, false if not.
		 */
        public bool HasComponent<T>()
        {
            return HasComponent(typeof(T));
        }
        public bool HasComponent(Type type)
        {
            return Components.ContainsKey(type);
        }
    }
}
