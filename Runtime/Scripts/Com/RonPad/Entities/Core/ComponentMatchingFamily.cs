// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/20
// *************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using Com.RonPad.Entities.Tools;
using Com.RonPad.Entities.Interfaces;
using Com.RonPad.Entities.Core;
using UnityEngine;
namespace Com.RonPad.Entities.Core
{
    /**
	 * The default class for managing a NodeList. This class creates the NodeList and adds and removes
	 * nodes to/from the list as the entities and the components in the engine change.
	 * 
	 * It uses the basic entity matching pattern of an entity system - entities are added to the list if
	 * they contain components matching all the public properties of the node class.
	 */
    public class ComponentMatchingFamily<T> : IFamily where T : Node
    {
        private NodeList<T> _nodeList;
        private Dictionary<Entity, T> _entities;
        private Dictionary<Type, string> _components;
        private NodePool<T> _nodePool;
        private IGameEngine _engine;
        private Type _type;

        /**
		 * The constructor. Creates a ComponentMatchingFamily to provide a NodeList for the
		 * given node class.
		 * 
		 * @param nodeClass The type of node to create and manage a NodeList for.
		 * @param engine The engine that this family is managing teh NodeList for.
		 */
        public ComponentMatchingFamily(IGameEngine engine)
        {
            _type = typeof(T);
            _engine = engine;
            Init();
        }

        /**
		 * Initialises the class. Creates the nodelist and other tools. Analyses the node to determine
		 * what component types the node requires.
		 */
        private void Init()
        {
            _nodeList = new NodeList<T>();
            _entities = new Dictionary<Entity, T>();
            _components = new Dictionary<Type, string>();
            _nodePool = new NodePool<T>(_components);

            _nodePool.Dispose(_nodePool.GetOne()); // create a dummy instance to ensure describeType works.

            CheckProperties();
            CheckFields();
        }

        private void CheckProperties()
        {
            var properties = _type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var info in properties)
            {
                if (info.Name.ToLower() == "entity")
                {
                    continue;
                }
                if (info.Name.ToLower() == "previous")
                {
                    continue;
                }
                if (info.Name.ToLower() == "next")
                {
                    continue;
                }
                var type = info.PropertyType;
                if (!_components.ContainsKey(type))
                {
                    _components.Add(type, info.Name);
                }
            }
        }
        private void CheckFields()
        {
            var fields = _type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var info in fields)
            {
                if (info.Name.ToLower() == "entity")
                {
                    continue;
                }
                if (info.Name.ToLower() == "previous")
                {
                    continue;
                }
                if (info.Name.ToLower() == "next")
                {
                    continue;
                }
                var type = info.FieldType;
                if (!_components.ContainsKey(type))
                {
                    _components.Add(type, info.Name);
                }
            }
        }
        /**
		 * The nodelist managed by this family. This is a reference that remains valid always
		 * since it is retained and reused by Systems that use the list. i.e. we never recreate the list,
		 * we always modify it in place.
		 */
        public object NodeList => _nodeList;

        /**
		 * Called by the engine when an entity has been added to it. We check if the entity should be in
		 * this family's NodeList and add it if appropriate.
		 */
        public void NewEntity(Entity entity)
        {
            AddIfMatch(entity);
        }

        /**
		 * Called by the engine when a component has been added to an entity. We check if the entity is not in
		 * this family's NodeList and should be, and add it if appropriate.
		 */
        public void ComponentAddedToEntity(Entity entity, Type type)
        {
            AddIfMatch(entity);
        }

        /**
		 * Called by the engine when a component has been removed from an entity. We check if the removed component
		 * is required by this family's NodeList and if so, we check if the entity is in this this NodeList and
		 * remove it if so.
		 */
        public void ComponentRemovedFromEntity(Entity entity, Type type)
        {
            if (_components.ContainsKey(type))
            {
                RemoveIfMatch(entity);
            }
        }

        /**
		 * Called by the engine when an entity has been rmoved from it. We check if the entity is in
		 * this family's NodeList and remove it if so.
		 */
        public void RemoveEntity(Entity entity)
        {
            RemoveIfMatch(entity);
        }

        /**
		 * If the entity is not in this family's NodeList, tests the components of the entity to see
		 * if it should be in this NodeList and adds it if so.
		 */
        private void AddIfMatch(Entity entity)
        {
            if (!_entities.ContainsKey(entity))
            {
                Type type;
                foreach (var item in _components)
                {
                    type = item.Key;
                    if (!entity.HasComponent(type))
                    {
                        return;
                    }
                }
                var node = _nodePool.GetOne();
                node.Entity = entity;
                foreach (var item in _components)
                {
                    type = item.Key;
                    var propertyName = _components[type];
                    var nodeType = node.GetType();
                    nodeType.SetTypeValue(node, propertyName, entity.GetComponent(type));
                }
                _entities.Add(entity, node);
                _nodeList.Add(node);
            }
        }

        /**
		 * Removes the entity if it is in this family's NodeList.
		 */
        private void RemoveIfMatch(Entity entity)
        {
            if (_entities.ContainsKey(entity))
            {
                var node = _entities[entity];
                _entities.Remove(entity);
                _nodeList.Remove(node);
                if (_engine.IsUpdating)
                {
                    _nodePool.Cache(node);
                    _engine.UpdateCompleted.Add(ReleaseNodePoolCache);
                }
                else
                {
                    _nodePool.Dispose(node);
                }
            }
        }

        /**
		 * Releases the nodes that were added to the node pool during this engine update, so they can
		 * be reused.
		 */
        private void ReleaseNodePoolCache()
        {
            _engine.UpdateCompleted.Remove(ReleaseNodePoolCache);
            _nodePool.ReleaseCache();
        }

        /**
		 * Removes all nodes from the NodeList.
		 */
        public void CleanUp()
        {
            for (var node = _nodeList.Head; node != null; node = (T)node.Next)
            {
                _entities.Remove(node.Entity);
            }
            _nodeList.RemoveAll();
        }
    }
}
