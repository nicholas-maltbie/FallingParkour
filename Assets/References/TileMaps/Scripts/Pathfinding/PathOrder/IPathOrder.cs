
using System;
using System.Collections.Generic;

namespace nickmaltbie.TileMap.Pathfinding.PathOrder
{
    /// <summary>
    /// Generic collection of paths used for pathfinding that can have paths of a specific type added to it
    /// or paths of a specific type removed from it.
    /// </summary>
    /// <typeparam name="V">Type of elements used in paths.</typeparam>
    public interface IPathOrder<V>
    {
        /// <summary>
        /// Add a path to the given collection.
        /// </summary>
        /// <param name="path">Path to add to this collection.</param>
        void AddPath(Path<V> path);

        /// <summary>
        /// Peek at the next path to check without removing it from the collection.
        /// </summary>
        /// <returns>The path that is stored at the front of the collection.</returns>
        /// <exception cref="IndexOutOfRangeException">If the collection is empty.</returns>
        Path<V> Peek();

        /// <summary>
        /// Get the next path to check and remove it from the collection.
        /// </summary>
        /// <returns>The path that is stored at the front of the collection.</returns>
        /// <exception cref="IndexOutOfRangeException">If the collection is empty.</returns>
        Path<V> Pop();

        /// <summary>
        /// Get the number of elements stored in this current path collection.
        /// </summary>
        int Count { get; }
    }
}
