// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/21
// *************************************************/

using System;
using Com.RonPad.Entities.Core;
using Com.RonPad.Entities.Interfaces;
namespace Com.RonPad.Entities.Tools
{
    /**
	 * A useful class for systems which simply iterate over a set of nodes, performing the same action on each node. This
	 * class removes the need for a lot of boilerplate code in such systems. Extend this class and pass the node type and
	 * a node update method into the constructor. The node update method will be called once per node on the update cycle
	 * with the node instance and the frame time as parameters. e.g.
	 * 
	 * <code>package
	 * {
	 *   public class MySystem extends ListIteratingSystem
	 *   {
	 *     public function MySystem()
	 *     {
	 *       super( MyNode, updateNode );
	 *     }
	 *     
	 *     private function updateNode( node : MyNode, time : Number ) : void
	 *     {
	 *       // process the node here
	 *     }
	 *   }
	 * }</code>
	 */
    public class ListIteratingSystem<T> : SystemBase where T : Node
    {
        protected NodeList<T> NodeList;
        protected Action<T, float> NodeUpdateFunction;
        protected Action<T> NodeAddedFunction;
        protected Action<T> NodeRemovedFunction;
        public ListIteratingSystem(Action<T, float> nodeUpdateFunction)
        {
            NodeUpdateFunction = nodeUpdateFunction;
        }
        public ListIteratingSystem(Action<T, float> nodeUpdateFunction, Action<T> nodeAddedFunction, Action<T> nodeRemovedFunction)
        {
            NodeUpdateFunction = nodeUpdateFunction;
            NodeAddedFunction = nodeAddedFunction;
            NodeRemovedFunction = nodeRemovedFunction;
        }
        public ListIteratingSystem()
        {

        }
        public override void AddToEngine(IGameEngine engine)
        {
            NodeList = engine.GetNodeList<T>();
            if (NodeAddedFunction != null)
            {
                for (var node = NodeList.Head; node != null; node = (T)node.Next)
                {
                    NodeAddedFunction(node);
                }
                NodeList.NodeAdded.Add(NodeAddedFunction);
            }
            if (NodeRemovedFunction != null)
            {
                NodeList.NodeRemoved.Add(NodeRemovedFunction);
            }
        }

        public override void RemoveFromEngine(IGameEngine engine)
        {
            if (NodeAddedFunction != null)
            {
                NodeList.NodeAdded.Remove(NodeAddedFunction);
            }
            if (NodeRemovedFunction != null)
            {
                NodeList.NodeRemoved.Remove(NodeRemovedFunction);
            }
            NodeList = null;
        }

        public override void Update(float time)
        {
            for (var node = NodeList.Head; node != null; node = (T)node.Next)
            {
                NodeUpdateFunction.Invoke(node, time);
            }
        }
    }
}
