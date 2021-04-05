using System.Collections.Generic;
using UnityEngine;

namespace PropHunt.Utils
{
    public class PrimitiveColliderCast : ColliderCast
    {
        private static Collider[] Empty = new Collider[] { };

        public LayerMask layerMask = 0;

        public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

        public RaycastHit[] GetHits(Vector3 direction, float distance)
        {
            CapsuleCollider capsuleCollider = this.GetComponent<CapsuleCollider>();
            SphereCollider sphereCollider = this.GetComponent<SphereCollider>();
            BoxCollider boxCollider = this.GetComponent<BoxCollider>();

            RaycastHit[] hits;
            if (sphereCollider != null)
            {
                hits = Physics.SphereCastAll(transform.position + sphereCollider.center, sphereCollider.radius, direction, distance);
            }
            else if (capsuleCollider != null)
            {
                Vector3 p1 = transform.position + capsuleCollider.center + transform.rotation * Vector3.down * (capsuleCollider.height * 0.5f - capsuleCollider.radius);
                Vector3 p2 = transform.position + capsuleCollider.center + transform.rotation * Vector3.up * (capsuleCollider.height * 0.5f - capsuleCollider.radius);
                hits = Physics.CapsuleCastAll(p1, p2,
                    capsuleCollider.radius, direction, distance, layerMask,
                    queryTriggerInteraction);
            }
            else if (boxCollider != null)
            {
                hits = Physics.BoxCastAll(transform.position + boxCollider.center,
                    boxCollider.size / 2, direction, transform.rotation, distance,
                    layerMask, queryTriggerInteraction);
            }
            else
            {
                hits = Physics.RaycastAll(transform.position, direction, distance,
                    layerMask, queryTriggerInteraction);
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

        public override IEnumerable<ColliderCastHit> GetOverlappingDirectional()
        {
            List<ColliderCastHit> hits = new List<ColliderCastHit>();
            foreach (RaycastHit hit in GetHits(Vector3.down, 0.001f))
            {
                if (hit.collider.gameObject.transform != gameObject.transform && hit.distance == 0)
                {
                    hits.Add(new ColliderCastHit
                    {
                        hit = true,
                        distance = 0,
                        fraction = 0,
                        normal = hit.normal,
                        pointHit = hit.point,
                        collider = hit.collider
                    });
                }
            }
            return hits;
        }

        public override IEnumerable<Collider> GetOverlapping()
        {
            CapsuleCollider capsuleCollider = this.GetComponent<CapsuleCollider>();
            SphereCollider sphereCollider = this.GetComponent<SphereCollider>();
            BoxCollider boxCollider = this.GetComponent<BoxCollider>();

            if (sphereCollider != null)
            {
                Collider[] overlaps = Physics.OverlapSphere(transform.position + sphereCollider.center, sphereCollider.radius, layerMask, queryTriggerInteraction);
                return overlaps;
            }
            else if (capsuleCollider != null)
            {
                Vector3 p1 = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5f;
                return Physics.OverlapCapsule(p1, p1 + Vector3.up * capsuleCollider.height,
                    capsuleCollider.radius, layerMask,
                    queryTriggerInteraction);
            }
            else if (boxCollider != null)
            {
                return Physics.OverlapBox(transform.position + boxCollider.center,
                    boxCollider.size, transform.rotation,
                    layerMask, queryTriggerInteraction);
            }
            else
            {
                return Empty;
            }
        }
    }
}