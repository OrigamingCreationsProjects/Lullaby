using UnityEngine;

namespace Lullaby
{
    public static class BoundsHelper
    {
        /// <summary>
        /// Returns true if the bounds of a collider is bellow a given point.
        /// </summary>
        /// <param name="collider">The collider you want to check.</param>
        /// <param name="point">The point in world space.</param>
        /// <returns></returns>
        public static bool IsBellowPoint(Collider collider, Vector3 point)
        {
            return false;
        }
    }
}