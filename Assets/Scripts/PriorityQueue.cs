using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace Nkg
{
    /// <summary>
    /// Priority queue implemented on max binary heap on List.
    /// For min priority queue, reverse implementation of Compare
    /// </summary>
    public class PriorityQueue<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Underlying list exposed so pathfinding can see what's in the queue.
        /// Should be used in conjuction with RemoveAt.
        /// </summary>
        public List<T> items;
        readonly Func<T, T, int> Compare;

        // Optimization for pathfinding, search open set a lot
        Dictionary<T, int> itemToIndex;
        bool needsReindexing;

        /// <summary>
        /// Initializes a new <see cref="T:Nkg.PriorityQueue`1"/> class.
        /// </summary>
        /// <param name="Comparator_in">Comparator function. If l &lt; r return -1 then r will be moved toward the front of the queue.</param>
        public PriorityQueue(Func<T, T, int> Comparator_in)
        {
            Compare = Comparator_in;
            items = new List<T>();
            itemToIndex = new Dictionary<T, int>();
            needsReindexing = true;
        }

        public int Count { get { return items.Count; } }

        /// <summary>
        /// Inserts an element.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Add(T item)
        {
            // Insert new item at end
            items.Add(item);
            int i = items.Count - 1;

            // Fix heap ordering property if violated
            FixUp(i);
            needsReindexing = true;
        }

        /// <summary>
        /// Fixes upward from a single out of place element. Element must not 
        /// already be higher than it should be!
        /// </summary>
        /// <param name="i">The index.</param>
        private void FixUp(int i)
        {
            while (i != 0)
            {
                int parentIdx = Parent(i);
                T self = items[i];
                T parent = items[parentIdx];
                if (Compare(parent, self) >= 0) break;

                items[parentIdx] = self;
                items[i] = parent;
                i = parentIdx;
            }
        }

        /// <summary>
        /// Replaces an item at underlying array index i.
        /// WARNING: New element must have higher or unchanged priority (or
        /// lower or unchanged priority, if a min priority queue is being used).
        /// Changing the key in the wrong direction will invalidate the priority
        /// queue!
        /// 
        /// This is exposed publicly to be used for optimization for A*
        /// when they want to change key values without removing and adding to
        /// the end of the array.
        /// </summary>
        public void ChangeKey(int i, T newItem)
        {
            Debug.Assert(Compare(items[i], newItem) <= 0);
            items[i] = newItem;
            FixUp(i);
            needsReindexing = true;
        }

        /// <summary>
        /// Returns the highest priority element.
        /// </summary>
        /// <returns>The highest priority element.</returns>
        public T Top() { return items[0]; }

        /// <summary>
        /// Removes and returns the highest priority element.
        /// </summary>
        /// <returns>The highest priority element.</returns>
        public T Pop()
        {
            Debug.Assert(items.Count > 0);
            T root = items[0];

            if (items.Count == 1)
            {
                items.RemoveAt(0);
                return root;
            }

            // Remove max heap element and resort
            items[0] = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            MaxHeapify(0);

            needsReindexing = true;
            return root;
        }

        /// <summary>
        /// Finds an item by value in the queue.
        /// </summary>
        /// <returns>(index, item). If not found, index = -1.</returns>
        /// <param name="finder">Finder.</param>
        // public (int, T) Find(Func<T, bool> finder)
        // {
        //     ReIndex();

        //     for (int i = 0; i < items.Count; ++i)
        //     {
        //         if (finder(items[i])) return (i, items[i]);
        //     }
        //     return (-1, default(T));
        // }

        public (int, T) FindIndex(T val)
        {
            if (needsReindexing)
            {
                ReIndex();
            }

            if (!itemToIndex.ContainsKey(val))
            {
                return (-1, default(T));
            }

            int idx = itemToIndex[val];
            return (idx, items[idx]);
        }

        /// <summary>
        /// Removes and returns element at i.
        /// </summary>
        /// <returns>The <see cref="!:T"/>.</returns>
        /// <param name="i">The index.</param>
        public T RemoveAt(int i)
        {
            // Move element at i to top of the heap while preserving surrounding order
            while (i != 0)
            {
                int parentIdx = Parent(i);
                items[i] = items[parentIdx];
                i = parentIdx;
            }

            needsReindexing = true;

            return Pop();
        }

        /// <summary>
        /// Recursive method to heapify starting at root index
        /// assuming both subtrees are presorted.
        /// </summary>
        /// <param name="index">Index.</param>
        private void MaxHeapify(int i)
        {
            int l = Left(i);
            int r = Right(i);
            int greatestIdx = i;
            T greatest = items[i];
            if (l < items.Count && Compare(items[l], greatest) > 0)
            {
                greatestIdx = l;
                greatest = items[l];
            }
            if (r < items.Count && Compare(items[r], greatest) > 0)
            {
                greatestIdx = r;
                greatest = items[r];
            }
            if (greatestIdx != i)
            {
                items[greatestIdx] = items[i];
                items[i] = greatest;
                MaxHeapify(greatestIdx);
            }
        }

        private void ReIndex()
        {
            itemToIndex.Clear();
            for (int i = 0; i < items.Count; ++i)
            {
                itemToIndex.Add(items[i], i);
            }
            needsReindexing = false;
        }

        // Traversal helpers
        private int Parent(int i) { return (i - 1) / 2; }
        private int Left(int i) { return 2 * i + 1; }
        private int Right(int i) { return 2 * i + 2; }
    }
}
