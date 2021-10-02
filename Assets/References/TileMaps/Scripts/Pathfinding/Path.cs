using System.Collections.Generic;
using System.Linq;
using nickmaltbie.TileMap.Common;

namespace nickmaltbie.TileMap.Pathfinding
{
    /// <summary>
    /// Represent a path via recursive definition for finding a path between nodes in a graph.
    /// </summary>
    /// <typeparam name="V">Type of nodes stored in the path</typeparam>
    public class Path<V>
    {
        /// <summary>
        /// Previous path.
        /// </summary>
        private readonly Path<V> previous;

        /// <summary>
        /// node stored at this location in the path.
        /// </summary>
        private readonly V node;

        /// <summary>
        /// Get the node stored at this step in the path.
        /// </summary>
        public V Node => node;

        /// <summary>
        /// Create a path that consists of just a single node that has no previous path.
        /// </summary>
        /// <param name="node">Single node within the path.</param>
        public Path(V node)
        {
            this.previous = null;
            this.node = node;
        }

        /// <summary>
        /// Create a path for a current node and with provided previous path.
        /// </summary>
        /// <param name="node">node stored at this step in the path.</param>
        /// <param name="previous">Previous nodes in the path.</param>
        public Path(V node, Path<V> previous)
        {
            this.previous = previous;
            this.node = node;
        }

        /// <summary>
        /// Get the full length of the path.
        /// </summary>
        public int Length()
        {
            int length = 1;
            Path<V> previous = this.previous;
            while (previous != null)
            {
                length += 1;
                previous = previous.previous;
            }
            return length;
        }

        /// <summary>
        /// Enumerate the full path from the start to this node (will end with this node).
        /// </summary>
        /// <returns>Enumerable of all nodes in the path</returns>
        public IEnumerable<V> FullPath()
        {
            Stack<V> stack = new Stack<V>();
            stack.Push(node);
            Path<V> previous = this.previous;
            while (previous != null)
            {
                stack.Push(previous.node);
                previous = previous.previous;
            }
            return stack.AsEnumerable();
        }
    }
}