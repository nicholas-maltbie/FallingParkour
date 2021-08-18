using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace PropHunt.Spectator
{
    /// <summary>
    /// An object that can be followed by a spectator player
    /// </summary>
    public class Followable : MonoBehaviour, IComparable<Followable>
    {
        /// <summary>
        /// Start each followable object with a random GUID id
        /// </summary>
        private Guid id = Guid.NewGuid();

        /// <summary>
        /// Get identifier for this followable object
        /// </summary>
        public Guid Id => id;

        public int CompareTo(Followable other)
        {
            return id.CompareTo(other.id);
        }
    }
}
