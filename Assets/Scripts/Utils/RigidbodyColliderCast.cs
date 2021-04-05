using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Utils
{
    public class RigidbodyColliderCast : ColliderCast
    {
        public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

        private List<Collider> overlapping = new List<Collider>();

        public RaycastHit[] GetHits(Vector3 direction, float distance)
        {
            RaycastHit[] hits = new RaycastHit[0];
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                hits = rigidbody.SweepTestAll(direction, distance, queryTriggerInteraction);
            }
            return hits;
        }

        public override ColliderCastHit CastSelf(Vector3 direction, float distance)
        {
            RaycastHit closest = new RaycastHit() { distance = Mathf.Infinity };
            bool hitSomething = false;
            foreach (RaycastHit hit in GetHits(direction, distance))
            {
                if (hit.collider.gameObject.transform != gameObject.transform)
                {
                    if (hit.distance < closest.distance)
                    {
                        closest = hit;
                    }
                    hitSomething = true;
                }
            }

            return new ColliderCastHit
            {
                hit = hitSomething,
                distance = closest.distance,
                pointHit = closest.point,
                normal = closest.normal,
                fraction = closest.distance / distance,
                collider = closest.collider
            };
        }

        public void OnCollisionEnter(Collision other)
        {
            overlapping.Add(other.collider);
        }

        public void OnCollisionExit(Collision other)
        {
            overlapping.Remove(other.collider);
        }

        public override IEnumerable<Collider> GetOverlapping()
        {
            return overlapping;
        }

        public override IEnumerable<ColliderCastHit> GetOverlappingDirectional()
        {
            List<ColliderCastHit> hits = new List<ColliderCastHit>();
            Collider collider = GetComponent<Collider>();
            var boundingBox = collider.bounds;
            foreach (Collider otherCollider in Physics.OverlapBox(transform.position + boundingBox.center, boundingBox.size / 2, Quaternion.identity, ~0, queryTriggerInteraction))
            {
                Physics.ComputePenetration(collider,
                    collider.transform.position, collider.transform.rotation, otherCollider,
                    otherCollider.transform.position, otherCollider.transform.rotation, out Vector3 direction, out float distance);
                if (otherCollider.gameObject != gameObject && distance > 0)
                {
                    hits.Add(new ColliderCastHit
                    {
                        hit = true,
                        distance = 0,
                        fraction = 0,
                        normal = direction.normalized,
                        pointHit = Vector3.zero,
                        collider = otherCollider
                    });
                }
            }
            return hits;
        }
    }
}