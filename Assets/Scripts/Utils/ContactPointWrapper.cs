
using UnityEngine;

namespace PropHunt.Utils
{
    public interface IContactPoint
    {
        /// <summary>
        /// The point of contact.
        /// </summary>
        Vector3 point { get; }

        /// <summary>
        /// Normal of the contact point.
        /// </summary>
        Vector3 normal { get; }

        /// <summary>
        /// The first collider in contact at the point.
        /// </summary>
        Collider thisCollider { get; }

        /// <summary>
        /// The other collider in contact at the point.
        /// </summary>
        Collider otherCollider { get; }

        /// <summary>
        /// The distance between the colliders at the contact point.
        /// </summary>
        float separation { get; }
    }

    public class ContactPointWrapper : IContactPoint
    {
        public static IContactPoint[] ConvertContactPoints(ContactPoint[] points)
        {
            IContactPoint[] converted = new IContactPoint[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                converted[i] = new ContactPointWrapper(points[i]);
            }
            return converted;
        }

        public ContactPointWrapper(ContactPoint contact)
        {
            this.contact = contact;
        }

        public ContactPoint contact;

        /// <summary>
        /// The point of contact.
        /// </summary>
        public Vector3 point => contact.point;

        /// <summary>
        /// Normal of the contact point.
        /// </summary>
        public Vector3 normal => contact.normal;

        /// <summary>
        /// The first collider in contact at the point.
        /// </summary>
        public Collider thisCollider => contact.thisCollider;

        /// <summary>
        /// The other collider in contact at the point.
        /// </summary>
        public Collider otherCollider => contact.otherCollider;

        /// <summary>
        /// The distance between the colliders at the contact point.
        /// </summary>
        public float separation => contact.separation;
    }
}
