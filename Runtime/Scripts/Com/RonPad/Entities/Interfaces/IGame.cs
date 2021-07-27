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
    public interface IGame : IDisposable
    {
        string Name { get; }
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
        void AddSystem(Core.System system, int priority);
        T GetSystem<T>();
        object GetSystem(Type type);
        Core.System[] GetSystems();
        void GetSystems(List<Core.System> results);
        void RemoveSystem(Core.System system);
        void RemoveAllSystems();
        void Pause();
        void Resume();
        void Start();
        void Update(float time);
        void LateUpdate(float time);
        void FixedUpdate(float time);
    }
}
