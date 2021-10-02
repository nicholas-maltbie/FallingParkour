
using System;
using System.Collections.Generic;

namespace nickmaltbie.TileMap.Common
{
    /// <summary>
    /// Heap data structure that can store any given item for a specific value. Supports storing a generic set of
    /// elements by a given key value where the element with the smallest value is stored at the root of the tree. This
    /// is an implementation of the min heap structure via an array. The array can be specified size when starting up
    /// but will grow if more elements are added.
    /// </summary>
    /// <typeparam name="K">Key used to sort elements, must implement IComparable<K>.</typeparam>
    /// <typeparam name="V">Type of data stored in the heap.</typeparam>
    public class Heap<K, V> where K : IComparable
    {
        /// <summary>
        /// Values stored in this heap.
        /// </summary>
        private (K, V)[] values;

        /// <summary>
        /// Get the current count of elements stored in the heap
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Create an instance of a Heap with an initial capacity of 10.
        /// </summary>
        public Heap() : this(10)
        {

        }

        /// <summary>
        /// Create an instance of a Heap with an initial capacity.
        /// </summary>
        /// <param name="initialCapacity">Starting capacity of the heap (can grow larger in future).</param>
        public Heap(int initialCapacity)
        {
            this.values = new (K, V)[initialCapacity];
            this.Count = 0;
        }

        /// <summary>
        /// Look at the first element stored in the heap.
        /// </summary>
        /// <returns>The value with the smallest key value stored at the base of the heap.</returns>
        /// <exception cref="IndexOutOfRangeException">If the heap does not have any elements stored.</returns>
        public V Peek()
        {
            if (Count > 0)
            {
                (K key, V value) = this.values[0];
                return value;
            }
            else
            {
                throw new IndexOutOfRangeException("Heap has no elements, cannot peek first element.");
            }
        }

        /// <summary>
        /// Adds an element to the heap for a given key.
        /// </summary>
        /// <param name="key">Key for the given element in the heap.</param>
        /// <param name="value">Value for the given key.</param>
        public void Add(K key, V value)
        {
            // Check if heap is at capacity
            if (this.values.Length == this.Count)
            {
                int newCapacity = this.values.Length * 2;
                if (newCapacity <= 0)
                {
                    newCapacity = 1;
                }
                // If so, double capacity
                (K, V)[] doubled = new (K, V)[newCapacity];
                System.Array.Copy(this.values, doubled, this.values.Length);
                this.values = doubled;
            }

            // Add element to end of heap, and push up
            this.values[this.Count] = (key, value);
            this.Count++;
            this.PushUp(this.Count - 1);
        }

        /// <summary>
        /// Dequeues the element at the root of the heap with the minimum key value and then ensures the heap has a
        /// valid structure. This will take at most log_2(n) time where n is the size of the heap. 
        /// </summary>
        /// <returns>The element stored at the base of the heap.</returns>
        /// <exception cref="IndexOutOfRangeException">If the heap does not have any elements stored.</returns>
        public V Pop()
        {
            V elem = Peek();
            this.Count--;

            // If there are elements in the heap, ensure the heap is valid.
            if (this.Count > 0)
            {
                // Take the element at the end of the heap, insert it at the root, and push it down
                var end = this.values[this.Count];
                this.values[0] = end;
                PushDown(0);

                // Clear out the data stored at the end (at least replace with default)
                this.values[this.Count] = (default(K), default(V));
            }

            return elem;
        }

        /// <summary>
        /// Gets the index of a node's parent. 
        /// </summary>
        /// <param name="index">Index of node to check.</param>
        /// <returns>The index within values of the specified node's parent.</returns>
        private int GetParent(int index)
        {
            return (index - 1) / 2;
        }

        /// <summary>
        /// Gets the two children indices of a given node.
        /// </summary>
        /// <param name="index">Index of node to identify children of.</param>
        /// <returns>Returns the two children nodes for a given node.</returns>
        private (int, int) GetChildren(int index)
        {
            return (index * 2 + 1, index * 2 + 2);
        }

        /// <summary>
        /// Enumerates the children of a given node.
        /// </summary>
        /// <param name="index">Index of node to identify children of.</param>
        /// <returns>Returns an enumerable of the left and right child values for a given node</returns>
        private IEnumerable<int> GetChildrenAsEnumerable(int index)
        {
            (int left, int right) = GetChildren(index);
            yield return left;
            yield return right;
            yield break;
        }

        /// <summary>
        /// Checks if a node for a given index is in this specified heap.
        /// </summary>
        /// <param name="index">Index of node to check.</param>
        /// <returns>True if there is a node in the heap with this given index, false otherwise.</returns>
        private bool IsInHeap(int index)
        {
            return index >= 0 && index < Count;
        }

        /// <summary>
        /// Push an element down the heap from a given position. Will push the element down the heap if the value at the
        /// specificed index is greater than the value of its children. This will always complete in at most O(log_2(n))
        /// time.
        /// </summary>
        /// <param name="index">Index of element to push down.</param>
        private void PushDown(int index)
        {
            int nodeIndex = index;
            bool pushDown = true;
            while (pushDown)
            {
                int minIndex = nodeIndex;
                (K minKey, V minValue) = this.values[nodeIndex];

                foreach (int childNode in this.GetChildrenAsEnumerable(nodeIndex))
                {
                    if (!IsInHeap(childNode))
                    {
                        continue;
                    }

                    (K childKey, V childValue) = this.values[childNode];
                    if (childKey.CompareTo(minKey) < 0)
                    {
                        minIndex = childNode;
                        minValue = childValue;
                        minKey = childKey;
                    }
                }

                if (minIndex != nodeIndex)
                {
                    this.values[minIndex] = this.values[nodeIndex];
                    this.values[nodeIndex] = (minKey, minValue);
                }
                else
                {
                    pushDown = false;
                }

                nodeIndex = minIndex;
            }
        }

        /// <summary>
        /// Push an element up the heap from a given position. Will push the element up the heap if the value at the
        /// specificed index is less than the value of its parent. This will always complete in at most O(log_2(n))
        /// time.
        /// </summary>
        /// <param name="index">Index of element to push down.</param>
        private void PushUp(int index)
        {
            int nodeIndex = index;
            bool pushUp = true;
            while (pushUp)
            {
                // If we are at root, nothing more to do
                if (nodeIndex == 0)
                {
                    return;
                }

                int parentIndex = this.GetParent(nodeIndex);
                (K nodeKey, V nodeValue) = this.values[nodeIndex];
                (K parentKey, V parentValue) = this.values[parentIndex];

                if (nodeKey.CompareTo(parentKey) < 0)
                {
                    this.values[parentIndex] = (nodeKey, nodeValue);
                    this.values[nodeIndex] = (parentKey, parentValue);
                }
                else
                {
                    pushUp = false;
                }

                nodeIndex = parentIndex;
            }
        }
    }
}
