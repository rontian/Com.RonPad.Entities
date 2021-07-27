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
        internal SystemBase Head;
        internal SystemBase Tail;

        internal void Add(SystemBase systemBase)
        {
            if (Head == null)
            {
                Head = Tail = systemBase;
                systemBase.Next = systemBase.Previous = null;
            }
            else
            {
                SystemBase node;
                for (node = Tail; node != null; node = node.Previous)
                {
                    if (node.Priority <= systemBase.Priority)
                    {
                        break;
                    }
                }
                if (node == Tail)
                {
                    if (Tail != null)
                    {
                        Tail.Next = systemBase;
                        systemBase.Previous = Tail;
                        systemBase.Next = null;
                    }
                    Tail = systemBase;
                }
                else if (node == null)
                {
                    systemBase.Next = Head;
                    systemBase.Previous = null;
                    Head.Previous = systemBase;
                    Head = systemBase;
                }
                else
                {
                    systemBase.Next = node.Next;
                    systemBase.Previous = node;
                    node.Next.Previous = systemBase;
                    node.Next = systemBase;
                }
            }
        }

        internal void Remove(SystemBase systemBase)
        {
            if (Head == systemBase)
            {
                Head = Head.Next;
            }
            if (Tail == systemBase)
            {
                Tail = Tail.Previous;
            }

            if (systemBase.Previous != null)
            {
                systemBase.Previous.Next = systemBase.Next;
            }

            if (systemBase.Next != null)
            {
                systemBase.Next.Previous = systemBase.Previous;
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

        internal SystemBase GetSystem(Type type)
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
