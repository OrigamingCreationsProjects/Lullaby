using System.Collections;
using System.Collections.Generic;
using Lullaby;
using UnityEngine;

namespace Lullaby
{
    [RequireComponent(typeof(WaypointManager))]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Lullaby/Misc/MovingPlatform")]
    public class MovingPlatform : MonoBehaviour
    {
        public float speed = 3f;
        
        public WaypointManager waypoints { get; protected set; }

        protected const float minDistance = 0.001f;

        protected virtual void Awake()
        {
            tag = GameTags.Platform;
            waypoints = GetComponent<WaypointManager>();
        }

        protected virtual void Update()
        {
            if (waypoints.index < waypoints.waypoints.Count)
            {
                var position = transform.position;
                var target = waypoints.current.position;
                position = Vector3.MoveTowards(position, target, speed * Time.deltaTime);
                transform.position = position;

                var distance = Vector3.Distance(transform.position, target);

                if (distance <= minDistance)
                    waypoints.Next();
            }
        }

        public void RestartRoute()
        {
            if (waypoints.mode == WaypointMode.Once && waypoints.routeFinished)
            {
                waypoints.routeFinished = false;
                transform.position = waypoints.waypoints[0].position;
                waypoints.ResetRoute();
            }
        }
    }
}