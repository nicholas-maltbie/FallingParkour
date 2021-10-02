
using System;
using System.Collections.Generic;

namespace nickmaltbie.TileMap.Pathfinding.PathOrder
{
    /// <summary>
    /// Path collection that uses a stack to store the next path to check with a first in last out methodology.
    /// </summary>
    /// <typeparam name="V">Type of elements used in paths.</typeparam>
    public class PathStack<V> : IPathOrder<V>
    {
        /// <summary>
        /// Collection of paths in this stack.
        /// </summary>
        private readonly Stack<Path<V>> stack;

        /// <summary>
        /// Initialize and instance of PathStack.
        /// </summary>
        public PathStack()
        {
            this.stack = new Stack<Path<V>>();
        }

        /// <inheritdoc/>
        public void AddPath(Path<V> path)
        {
            this.stack.Push(path);
        }

        /// <inheritdoc/>
        public Path<V> Peek()
        {
            return this.stack.Peek();
        }

        /// <inheritdoc/>
        public Path<V> Pop()
        {
            return this.stack.Pop();
        }

        /// <inheritdoc/>
        public int Count => this.stack.Count;
    }
}
