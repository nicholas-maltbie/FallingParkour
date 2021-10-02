
using System;
using System.Collections.Generic;
using nickmaltbie.TileMap.Common;

namespace nickmaltbie.TileMap.Pathfinding.PathOrder
{
    /// <summary>
    /// Path collection that uses weighted paths to store the next path in the collection via a function.
    /// These paths are sorted using a priority queue.
    /// </summary>
    /// <typeparam name="V">Type of elements used in paths.</typeparam>
    /// <typeparam name="W">Weighted elements of paths in priority queue.</typeparam>
    public class PathPriorityQueue<W, V> : IPathOrder<V> where W : IComparable
    {
        /// <summary>
        /// Collection of paths in this priority queue.
        /// </summary>
        private readonly Heap<W, Path<V>> pQueue;

        /// <summary>
        /// Get the weight of a path.
        /// </summary>
        private readonly Func<Path<V>, W> GetWeight;

        /// <summary>
        /// Initialize and instance of PathPriorityQueue with a given weighting function and an initial capacity of 10.
        /// </summary>
        /// <param name="GetWeight">Function to determine the weight of a given path.</param>
        public PathPriorityQueue(Func<Path<V>, W> GetWeight) : this(GetWeight, 10) { }

        /// <summary>
        /// Initialize and instance of PathPriorityQueue with a given weighting function and capacity.
        /// </summary>
        /// <param name="GetWeight">Function to determine the weight of a given path.</param>
        /// <param name="initialCapacity">Initial capacity of the path.</param>
        public PathPriorityQueue(Func<Path<V>, W> GetWeight, int initialCapacity)
        {
            this.GetWeight = GetWeight;
            this.pQueue = new Heap<W, Path<V>>(initialCapacity);
        }

        /// <inheritdoc/>
        public void AddPath(Path<V> path)
        {
            this.pQueue.Add(GetWeight(path), path);
        }

        /// <inheritdoc/>
        public Path<V> Peek()
        {
            return this.pQueue.Peek();
        }

        /// <inheritdoc/>
        public Path<V> Pop()
        {
            return this.pQueue.Pop();
        }

        /// <inheritdoc/>
        public int Count => this.pQueue.Count;
    }
}
