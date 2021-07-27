// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/20
// *************************************************/

using System;
using System.Collections.Generic;
using Com.RonPad.Entities.Interfaces;
using Com.RonPad.Entities.Tools;
using Com.RonPad.Signals;
namespace Com.RonPad.Entities.Core
{
    public class GameEngine : IGameEngine
    {
        public string Name { get; }
        public Type FamilyType { get; set; }
        public bool IsUpdating { get; private set; }
        public Signal0 UpdateCompleted { get; } = new Signal0();
        public bool IsRunning { get; private set; }

        private Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();
        private Dictionary<Type, IFamily> _families = new Dictionary<Type, IFamily>();
        private EntityList _entityList = new EntityList();
        private SystemList _systemList = new SystemList();
        /**
		 * The class used to manage node lists. In most cases the default class is sufficient
		 * but it is exposed here so advanced developers can choose to create and use a 
		 * different implementation.
		 * 
		 * The class must implement the Family interface.
		 */
        public GameEngine(string name, Type familyType = null)
        {
            Name = name;
            FamilyType = familyType;
        }
        public void FixedUpdate(float time)
        {
            for (var system = _systemList.Head; system != null; system = system.Next)
            {
                system.FixedUpdate(time);
            }
        }
        public void Update(float time)
        {
            IsUpdating = true;
            for (var system = _systemList.Head; system != null; system = system.Next)
            {
                system.Update(time);
            }
            IsUpdating = false;
            UpdateCompleted.Dispatch();
        }
        public void LateUpdate(float time)
        {
            for (var system = _systemList.Head; system != null; system = system.Next)
            {
                system.LateUpdate(time);
            }
        }

        /**
		 * Add an entity to the engine.
		 * 
		 * @param entity The entity to add.
		 */
        public void AddEntity(Entity entity)
        {
            if (_entities.ContainsKey(entity.Name))
            {
                throw new Exception($"The entity name {entity.Name}  is already in use by another entity.");
            }
            _entityList.Add(entity);
            _entities.Add(entity.Name, entity);
            entity.ComponentAdded.Add(ComponentAdded);
            entity.ComponentRemoved.Add(ComponentRemoved);
            entity.NameChanged.Add(EntityNameChanged);
            foreach (var family in _families)
            {
                family.Value.NewEntity(entity);
            }
        }
        /**
		 * Remove an entity from the engine.
		 * 
		 * @param entity The entity to remove.
		 */
        public void RemoveEntity(Entity entity)
        {
            entity.ComponentAdded.Remove(ComponentAdded);
            entity.ComponentRemoved.Remove(ComponentRemoved);
            entity.NameChanged.Remove(EntityNameChanged);
            foreach (var family in _families)
            {
                family.Value.RemoveEntity(entity);
            }
            _entities.Remove(entity.Name);
            _entityList.Remove(entity);
        }
        /**
		 * Get an entity based n its name.
		 * 
		 * @param name The name of the entity
		 * @return The entity, or null if no entity with that name exists on the engine
		 */
        public Entity GetEntity(string name)
        {
            return _entities.TryGetValue(name, out var entity) ? entity : null;
        }
        /**
		 * Remove all entities from the engine.
		 */
        public void RemoveAllEntities()
        {
            while (_entityList.Head != null)
            {
                RemoveEntity(_entityList.Head);
            }
        }
        /**
		 * Returns a vector containing all the entities in the engine.
		 */
        public Entity[] GetEntities()
        {
            var results = new List<Entity>();
            for (var entity = _entityList.Head; entity != null; entity = entity.Next)
            {
                results.Add(entity);
            }
            return results.ToArray();
        }
        /**
		 * Returns a vector containing all the entities in the engine.
		 */
        public void GetEntities(List<Entity> results)
        {
            if (results == null)
            {
                throw new Exception("results is invalid.");
            }
            results.Clear();
            for (var entity = _entityList.Head; entity != null; entity = entity.Next)
            {
                results.Add(entity);
            }
        }
        /**
		 * Get a collection of nodes from the engine, based on the type of the node required.
		 * 
		 * <p>The engine will create the appropriate NodeList if it doesn't already exist and 
		 * will keep its contents up to date as entities are added to and removed from the
		 * engine.</p>
		 * 
		 * <p>If a NodeList is no longer required, release it with the releaseNodeList method.</p>
		 * 
		 * @param type The type of node required.
		 * @return A linked list of all nodes of this type from all entities in the engine.
		 */
        public NodeList<T> GetNodeList<T>() where T : Node
        {
            return (NodeList<T>)GetNodeList(typeof(T));
        }
        public object GetNodeList(Type type)
        {
            FamilyType ??= typeof(ComponentMatchingFamily<>);
            if (_families.TryGetValue(type, out var family))
            {
                return family.NodeList;
            }
            var args = new object[1] {
                this
            };
            var types = new[] {
                type
            };
            family = FamilyType.CreateInstance(types, args) as IFamily;
            if (family != null)
            {
                _families.Add(type, family);
                for (var entity = _entityList.Head; entity != null; entity = entity.Next)
                {
                    family.NewEntity(entity);
                }
                return family.NodeList;
            }
            return null;
        }
        /**
		 * If a NodeList is no longer required, this method will stop the engine updating
		 * the list and will release all references to the list within the framework
		 * classes, enabling it to be garbage collected.
		 * 
		 * <p>It is not essential to release a list, but releasing it will free
		 * up memory and processor resources.</p>
		 * 
		 * @param type The type of the node class if the list to be released.
		 */
        public void ReleaseNodeList<T>()
        {
            ReleaseNodeList(typeof(T));
        }
        public void ReleaseNodeList(Type type)
        {
            if (_families.TryGetValue(type, out var family))
            {
                family.CleanUp();
            }
            _families.Remove(type);
        }
        /**
		 * Add a system to the engine, and set its priority for the order in which the
		 * systems are updated by the engine update loop.
		 * 
		 * <p>The priority dictates the order in which the systems are updated by the engine update 
		 * loop. Lower numbers for priority are updated first. i.e. a priority of 1 is 
		 * updated before a priority of 2.</p>
		 * 
		 * @param system The system to add to the engine.
		 * @param priority The priority for updating the systems during the engine loop. A 
		 * lower number means the system is updated sooner.
		 */
        public void AddSystem(SystemBase system, int priority)
        {
            system.Priority = priority;
            system.AddToEngine(this);
            _systemList.Add(system);
        }
        /**
		 * Get the system instance of a particular type from within the engine.
		 * 
		 * @param type The type of system
		 * @return The instance of the system type that is in the engine, or
		 * null if no systems of this type are in the engine.
		 */
        public T GetSystem<T>()
        {
            return (T)GetSystem(typeof(T));
        }
        public object GetSystem(Type type)
        {
            return _systemList.GetSystem(type);
        }
        public SystemBase[] GetSystems()
        {
            var results = new List<SystemBase>();
            for (var system = _systemList.Head; system != null; system = system.Next)
            {
                results.Add(system);
            }
            return results.ToArray();
        }
        public void GetSystems(List<SystemBase> results)
        {
            if (results == null)
            {
                throw new Exception("results is invalid.");
            }
            results.Clear();
            for (var system = _systemList.Head; system != null; system = system.Next)
            {
                results.Add(system);
            }
        }
        /**
		 * Remove a system from the engine.
		 * 
		 * @param system The system to remove from the engine.
		 */
        public void RemoveSystem(SystemBase system)
        {
            _systemList.Remove(system);
            system.RemoveFromEngine(this);
        }
        /**
		 * Remove all systems from the engine.
		 */
        public void RemoveAllSystems()
        {
            while (_systemList.Head != null)
            {
                var system = _systemList.Head;
                _systemList.Head = _systemList.Head.Next;
                system.Previous = null;
                system.Next = null;
                system.RemoveFromEngine(this);
            }
            _systemList.Tail = null;
        }
        public void Pause()
        {
            IsRunning = false;
        }
        public void Resume()
        {
            IsRunning = true;
        }
        public void Start()
        {
            IsRunning = true;
        }
        public void Dispose()
        {
            IsRunning = false;
            RemoveAllEntities();
            RemoveAllSystems();
            FamilyType = null;
        }

        #region Private Method
        private void ComponentAdded(Entity entity, Type type)
        {
            foreach (var family in _families)
            {
                family.Value.ComponentAddedToEntity(entity, type);
            }
        }

        private void ComponentRemoved(Entity entity, Type type)
        {
            foreach (var family in _families)
            {
                family.Value.ComponentRemovedFromEntity(entity, type);
            }
        }
        private void EntityNameChanged(Entity entity, string oldName)
        {
            if (_entities[oldName] == entity)
            {
                _entities.Remove(oldName);
                _entities.Add(entity.Name, entity);
            }
        }
        #endregion

    }
}
