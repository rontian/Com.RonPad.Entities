// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/27
// *************************************************/

using System;
using System.Collections.Generic;
using Com.RonPad.Signals;
using Com.RonPad.Entities.Core;
namespace Com.RonPad.Entities.Interfaces
{
    public interface IGameEngine : IDisposable
    {
        string Name { get; }
        bool IsRunning { get; }
        bool IsUpdating { get; }
        Signal0 UpdateCompleted { get; }
        void AddEntity(Entity entity);
        void RemoveEntity(Entity entity);
        Entity GetEntity(string name);
        void RemoveAllEntities();
        Entity[] GetEntities();
        void GetEntities(List<Entity> results);
        NodeList<T> GetNodeList<T>() where T : Node;
        object GetNodeList(Type type);
        void ReleaseNodeList<T>();
        void ReleaseNodeList(Type type);
        void AddSystem(SystemBase system, int priority);
        T GetSystem<T>();
        object GetSystem(Type type);
        SystemBase[] GetSystems();
        void GetSystems(List<SystemBase> results);
        void RemoveSystem(SystemBase system);
        void RemoveAllSystems();
        void Pause();
        void Resume();
        void Start();
        void Update(float time);
        void LateUpdate(float time);
        void FixedUpdate(float time);
    }
}
