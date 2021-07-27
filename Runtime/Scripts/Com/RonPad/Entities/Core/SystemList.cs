// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/20
// *************************************************/

using System;
namespace Com.RonPad.Entities.Core
{
    /**
	 * Used internally, this is an ordered list of Systems for use by the engine update loop.
	 */
    public class SystemList
    {
        internal System Head;
        internal System Tail;

        internal void Add(System system)
        {
            if (Head == null)
            {
                Head = Tail = system;
                system.Next = system.Previous = null;
            }
            else
            {
                System node;
                for (node = Tail; node != null; node = node.Previous)
                {
                    if (node.Priority <= system.Priority)
                    {
                        break;
                    }
                }
                if (node == Tail)
                {
                    if (Tail != null)
                    {
                        Tail.Next = system;
                        system.Previous = Tail;
                        system.Next = null;
                    }
                    Tail = system;
                }
                else if (node == null)
                {
                    system.Next = Head;
                    system.Previous = null;
                    Head.Previous = system;
                    Head = system;
                }
                else
                {
                    system.Next = node.Next;
                    system.Previous = node;
                    node.Next.Previous = system;
                    node.Next = system;
                }
            }
        }

        internal void Remove(System system)
        {
            if (Head == system)
            {
                Head = Head.Next;
            }
            if (Tail == system)
            {
                Tail = Tail.Previous;
            }

            if (system.Previous != null)
            {
                system.Previous.Next = system.Next;
            }

            if (system.Next != null)
            {
                system.Next.Previous = system.Previous;
            }
            // N.B. Don't set system.next and system.previous to null because that will break the list iteration if node is the current node in the iteration.
        }

        internal void RemoveAll()
        {
            while (Head != null)
            {
                var system = Head;
                Head = Head.Next;
                system.Previous = null;
                system.Next = null;
            }
            Tail = null;
        }

        internal System GetSystem(Type type)
        {
            for (var system = Head; system != null; system = system.Next)
            {
                if (system.GetType() == type)
                {
                    return system;
                }
            }
            return null;
        }
    }
}
