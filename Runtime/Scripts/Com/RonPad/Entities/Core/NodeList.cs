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
	 * A collection of nodes.
	 * 
	 * <p>Systems within the engine access the components of entities via NodeLists. A NodeList contains
	 * a node for each Entity in the engine that has all the components required by the node. To iterate
	 * over a NodeList, start from the head and step to the next on each loop, until the returned value
	 * is null.</p>
	 * 
	 * <p>for( var node : Node = nodeList.head; node; node = node.next )
	 * {
	 *   // do stuff
	 * }</p>
	 * 
	 * <p>It is safe to remove items from a nodelist during the loop. When a Node is removed form the 
	 * NodeList it's previous and next properties still point to the nodes that were before and after
	 * it in the NodeList just before it was removed.</p>
	 */
    public class NodeList<T> where T : Node
    {
        /**
		 * The first item in the node list, or null if the list contains no nodes.
		 */
        public T Head;
        /**
		 * The last item in the node list, or null if the list contains no nodes.
		 */
        public T Tail;

        /**
		 * A signal that is dispatched whenever a node is added to the node list.
		 * 
		 * <p>The signal will pass a single parameter to the listeners - the node that was added.</p>
		 */
        public readonly Signal1<T> NodeAdded = new Signal1<T>();
        /**
		 * A signal that is dispatched whenever a node is removed from the node list.
		 * 
		 * <p>The signal will pass a single parameter to the listeners - the node that was removed.</p>
		 */
        public readonly Signal1<T> NodeRemoved = new Signal1<T>();

        public NodeList()
        {
        }

        internal void Add(T node)
        {
            if (Head == null)
            {
                Head = Tail = node;
                node.Next = node.Previous = null;
            }
            else
            {
                Tail.Next = node;
                node.Previous = Tail;
                node.Next = null;
                Tail = node;
            }
            NodeAdded.Dispatch(node);
        }

        internal void Remove(T node)
        {
            if (Head == node)
            {
                Head = (T)Head.Next;
            }
            if (Tail == node)
            {
                Tail = (T)Tail.Previous;
            }

            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }

            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
            NodeRemoved.Dispatch(node);
            // N.B. Don't set node.next and node.previous to null because that will break the list iteration if node is the current node in the iteration.
        }

        internal void RemoveAll()
        {
            while (Head != null)
            {
                var node = Head;
                Head = (T)node.Next;
                node.Previous = null;
                node.Next = null;
                NodeRemoved.Dispatch(node);
            }
            Tail = null;
        }

        /**
		 * true if the list is empty, false otherwise.
		 */
        public bool IsEmpty => Head == null;

        /**
		 * Swaps the positions of two nodes in the list. Useful when sorting a list.
		 */
        public void Swap(Node node1, Node node2)
        {
            if (node1.Previous == node2)
            {
                node1.Previous = node2.Previous;
                node2.Previous = node1;
                node2.Next = node1.Next;
                node1.Next = node2;
            }
            else if (node2.Previous == node1)
            {
                node2.Previous = node1.Previous;
                node1.Previous = node2;
                node1.Next = node2.Next;
                node2.Next = node1;
            }
            else
            {
                var temp = node1.Previous;
                node1.Previous = node2.Previous;
                node2.Previous = temp;
                temp = node1.Next;
                node1.Next = node2.Next;
                node2.Next = temp;
            }
            if (Head == node1)
            {
                Head = (T)node2;
            }
            else if (Head == node2)
            {
                Head = (T)node1;
            }
            if (Tail == node1)
            {
                Tail = (T)node2;
            }
            else if (Tail == node2)
            {
                Tail = (T)node1;
            }
            if (node1.Previous != null)
            {
                node1.Previous.Next = node1;
            }
            if (node2.Previous != null)
            {
                node2.Previous.Next = node2;
            }
            if (node1.Next != null)
            {
                node1.Next.Previous = node1;
            }
            if (node2.Next != null)
            {
                node2.Next.Previous = node2;
            }
        }

        /**
		 * Performs an insertion sort on the node list. In general, insertion sort is very efficient with short lists 
		 * and with lists that are mostly sorted, but is inefficient with large lists that are randomly ordered.
		 * 
		 * <p>The sort function takes two nodes and returns a Number.</p>
		 * 
		 * <p><code>function sortFunction( node1 : MockNode, node2 : MockNode ) : Number</code></p>
		 * 
		 * <p>If the returned number is less than zero, the first node should be before the second. If it is greater
		 * than zero the second node should be before the first. If it is zero the order of the nodes doesn't matter
		 * and the original order will be retained.</p>
		 * 
		 * <p>This insertion sort implementation runs in place so no objects are created during the sort.</p>
		 */
        public void InsertionSort(Func<Node, Node, int> sortFunction)
        {
            if (Head == Tail)
            {
                return;
            }
            var remains = Head.Next;
            for (var node = remains;
                node != null;
                node = remains)
            {
                remains = node.Next;
                Node other;
                for (other = node.Previous;
                    other != null;
                    other = other.Previous)
                {
                    if (sortFunction.Invoke(node, other) >= 0)
                    {
                        // move node to after other
                        if (node != other.Next)
                        {
                            // remove from place
                            if (Tail == node)
                            {
                                Tail = (T)node.Previous;
                            }
                            node.Previous.Next = node.Next;
                            if (node.Next != null)
                            {
                                node.Next.Previous = node.Previous;
                            }
                            // insert after other
                            node.Next = other.Next;
                            node.Previous = other;
                            node.Next.Previous = node;
                            other.Next = node;
                        }
                        break; // exit the inner for loop
                    }
                }
                if (other == null) // the node belongs at the start of the list
                {
                    // remove from place
                    if (Tail == node)
                    {
                        Tail = (T)node.Previous;
                    }
                    node.Previous.Next = node.Next;
                    if (node.Next != null)
                    {
                        node.Next.Previous = node.Previous;
                    }
                    // insert at head
                    node.Next = Head;
                    Head.Previous = node;
                    node.Previous = null;
                    Head = (T)node;
                }
            }
        }

        /**
		 * Performs a merge sort on the node list. In general, merge sort is more efficient than insertion sort
		 * with long lists that are very unsorted.
		 * 
		 * <p>The sort function takes two nodes and returns a Number.</p>
		 * 
		 * <p><code>function sortFunction( node1 : MockNode, node2 : MockNode ) : Number</code></p>
		 * 
		 * <p>If the returned number is less than zero, the first node should be before the second. If it is greater
		 * than zero the second node should be before the first. If it is zero the order of the nodes doesn't matter.</p>
		 * 
		 * <p>This merge sort implementation creates and uses a single Vector during the sort operation.</p>
		 */
        public void MergeSort(Func<Node, Node, int> sortFunction)
        {
            if (Head == Tail)
            {
                return;
            }
            var lists = new LinkedList<Node>();
            // disassemble the list
            var start = Head;
            Node end;
            while (start != null)
            {
                end = start;
                while (end.Next != null && sortFunction.Invoke(end, end.Next) <= 0)
                {
                    end = end.Next;
                }
                var next = end.Next;
                start.Previous = end.Next = null;
                lists.AddLast(start);
                start = (T)next;
            }
            // reassemble it in order
            while (lists.Count > 1)
            {
                var h1 = lists.First.Value;
                lists.RemoveFirst();
                var h2 = lists.First.Value;
                lists.RemoveFirst();
                lists.AddLast(Merge(h1, h2, sortFunction));
            }
            // find the tail
            Tail = Head = (T)lists.First.Value;
            while (Tail.Next != null)
            {
                Tail = (T)Tail.Next;
            }
        }

        private Node Merge(Node head1, Node head2, Func<Node, Node, int> sortFunction)
        {
            Node node;
            Node head;
            if (sortFunction(head1, head2) <= 0)
            {
                head = node = head1;
                head1 = head1.Next;
            }
            else
            {
                head = node = head2;
                head2 = head2.Next;
            }
            while (head1 != null && head2 != null)
            {
                if (sortFunction(head1, head2) <= 0)
                {
                    node.Next = head1;
                    head1.Previous = node;
                    node = head1;
                    head1 = head1.Next;
                }
                else
                {
                    node.Next = head2;
                    head2.Previous = node;
                    node = head2;
                    head2 = head2.Next;
                }
            }
            if (head1 != null)
            {
                node.Next = head1;
                head1.Previous = node;
            }
            else
            {
                node.Next = head2;
                head2.Previous = node;
            }
            return head;
        }
    }
}
