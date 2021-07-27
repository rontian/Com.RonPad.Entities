// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/20
// *************************************************/

namespace Com.RonPad.Entities.Core
{
    /**
	 * An internal class for a linked list of entities. Used inside the framework for
	 * managing the entities.
	 */
    internal class EntityList
    {
        internal Entity Head;
        internal Entity Tail;

        internal void Add(Entity entity)
        {
            if (Head == null)
            {
                Head = Tail = entity;
                entity.Next = entity.Previous = null;
            }
            else
            {
                Tail.Next = entity;
                entity.Previous = Tail;
                entity.Next = null;
                Tail = entity;
            }
        }

        internal void Remove(Entity entity)
        {
            if (Head == entity)
            {
                Head = Head.Next;
            }
            if (Tail == entity)
            {
                Tail = Tail.Previous;
            }
            if (entity.Previous != null)
            {
                entity.Previous.Next = entity.Next;
            }
            if (entity.Next != null)
            {
                entity.Next.Previous = entity.Previous;
            }
            // N.B. Don't set entity.next and entity.previous to null because that will break the list iteration if node is the current node in the iteration.
        }

        internal void RemoveAll()
        {
            while (Head != null)
            {
                var entity = Head;
                Head = Head.Next;
                entity.Previous = null;
                entity.Next = null;
            }
            Tail = null;
        }
    }
}
