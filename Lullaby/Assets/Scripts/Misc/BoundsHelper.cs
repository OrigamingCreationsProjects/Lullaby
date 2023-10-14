using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Lullaby
{
    public static class BoundsHelper
    {
        private static Dictionary<Collider, Bounds> _localBounds = new(); // Create a dictionary to store the bounds of a collider without rotation.
        
        /// <summary>
        /// Returns the Bounds of a given collider without rotation.
        /// </summary>
        /// <param name="collider">The collider you want to get the Bounds from.</param>
        public static Bounds GetLocalBounds(Collider collider)
        {
            if (!_localBounds.ContainsKey(collider))
            {
                var originalRotation = collider.transform.rotation; // Store the original rotation of the collider.
                collider.transform.rotation = Quaternion.identity; // Set the rotation of the collider to zero.
                var localBounds = collider.bounds; // Get the bounds of the collider.
                collider.transform.rotation = originalRotation; // Reset the rotation of the collider.
                _localBounds.Add(collider, localBounds); 
            }
            return _localBounds[collider];
        }
        
        /// <summary>
        /// Returns true if the bounds of a collider is bellow a given point. 
        /// </summary>
        /// <param name="collider">The collider you want to check.</param>
        /// <param name="point">The point in world space.</param>
        public static bool IsBellowPoint(Collider collider, Vector3 point)
        {
            var localBounds = GetLocalBounds(collider); // Get the bounds of the collider.
            var extents = localBounds.extents.y; // Get the extents of the collider on the y axis.
            var top = collider.bounds.center + collider.transform.up * extents; // Get the top of the collider.
            
            var localPoint = collider.transform.InverseTransformPoint(point); // Convert the point to local space.
            var localTop = collider.transform.InverseTransformPoint(top); // Convert the top of the collider to local space.
            
            return  localPoint.y >= localTop.y; // Return true if the point is above the top of the collider.
                                                // We want to know if the collider's bounds is below the point
        }

        /// <summary>
        /// Returns true if the bounds of a collider is above a given point.
        /// </summary>
        /// <param name="collider">The collider you want to check.</param>
        /// <param name="point">The point in world space.</param>
        /// <returns></returns>
        public static bool IsAbovePoint(Collider collider, Vector3 point)
        {
            var localBounds = GetLocalBounds(collider);
            var extents = localBounds.extents.y;
            var top = collider.bounds.center + collider.transform.up * extents;
            
            var localPoint = collider.transform.InverseTransformPoint(point);
            var localTop = collider.transform.InverseTransformPoint(top);
            
            return  localPoint.y <= localTop.y; // Return true if the point is below the top of the collider.
                                                // We want to know if the collider's bounds is above the point
        }

        /// <summary>
        /// Returns true if a given point is bellow the top of the collider.
        /// </summary>
        /// <param name="collider">The collider you want to check.</param>
        /// <param name="point">The point in world space.</param>
        public static bool IsPointBellowTop(Collider collider, Vector3 point)
        {
            var localBounds = GetLocalBounds(collider);
            var extents = localBounds.extents.y;
            var top = collider.bounds.center + collider.transform.up * extents;
            
            var localPoint = collider.transform.InverseTransformPoint(point);
            var localTop = collider.transform.InverseTransformPoint(top);
            
            return localPoint.y <= localTop.y; // Return true if the point is below the top of the collider.
                                               // We want to know if the point is below the top of the collider.
        }

        /// <summary>
        /// Returns true if a given point is in the extends radius of a collider.
        /// </summary>
        /// <param name="collider">The collider you want to check.</param>
        /// <param name="point">The point in world space.</param>
        /// <returns></returns>
        public static bool IsPointInExtentsRadius(Collider collider, Vector3 point)
        {
            var head = collider.bounds.center - point;
            return head.magnitude <= collider.bounds.extents.x; // Si la distancia al punto es menor que el radio del collider,
                                                                // el punto está dentro del collider.
        }
        
        /// <summary>
        /// Returns true if a given point is inside a capsule.
        /// </summary>
        /// <param name="collider">The collider you want to check.</param>
        /// <param name="point">The reference point in world space.</param>
        /// <returns></returns>
        public static bool IsPointInsideCapsule(CapsuleCollider collider, Vector3 point)
        {
            var localBounds = GetLocalBounds(collider);
            var closestPoint = GetClosestPointFromCapsule(collider, point);
            var distance = (point - closestPoint).magnitude;

            return distance <= localBounds.extents.x; // Si la distancia al punto es menor que el radio del collider,
                                                      // el punto está dentro del collider.
        }
        /// <summary>
        /// Returns the closest position from a given point in a capsule.
        /// </summary>
        /// <param name="collider">The collider of the capsule.</param>
        /// <param name="point">The reference point in world space.</param>
        public static Vector3 GetClosestPointFromCapsule(CapsuleCollider collider, Vector3 point)
        {
            var localBounds = GetLocalBounds(collider);
            var center = collider.bounds.center;
            var capsuleOffset = localBounds.extents.y - localBounds.extents.x; // Get the offset of the capsule.
            var top = center + collider.transform.up * capsuleOffset; // Get the top of the capsule.
            var bottom = center - collider.transform.up * capsuleOffset; // Get the bottom of the capsule.
            
            return NearestPointOnFiniteLine(top, bottom, point); // Get the closest point from the point to the capsule.
        }
        
        /// <summary>
        /// Returns the nearest position from a given point in a line.
        /// </summary>
        /// <param name="start">The start of the line in world space.</param>
        /// <param name="end">The end of the line in world space.</param>
        /// <param name="point">The reference point in world space.</param>
        public static Vector3 NearestPointOnFiniteLine(Vector3 start, Vector3 end, Vector3 point)
        {
            var line = end - start;
            var len = line.magnitude;
            
            line /= len; // Normalize the line.
            
            var v = point - start;
            var d = Vector3.Dot(v, line); // Get the distance from the start of the line to the point.
            d = Mathf.Clamp(d, 0f, len); // Clamp the distance between 0 and the length of the line.
            
            return start + line * d; // Return the closest point from the point to the line.
        }

        #region -- NEAREST POINT ON SURFACE GETTERS --
        
        // DEJAMOS PROTOTIPADOS ESTOS METODOS POR SI FINALMENTE LOS NECESITAMOS Y ME FUMO UN PORRO CON CAMPOS DE GRAVEDAD
        public static Vector3 NearestPointOnBox(Vector3 center, Vector3 size, Quaternion rotation, Vector3 point)
        {
            throw new NotImplementedException();
        }

        public static Vector3 NearestPointOnDisc(Vector3 center, Vector3 normal, float radius, Vector3 point)
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}