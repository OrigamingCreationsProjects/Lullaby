using UnityEngine;

namespace Lullaby
{
    public enum WaypointMode
    {
        [Tooltip("Los waypoints seran recorridos en sentido circular.")]
        Loop, 
        [Tooltip("Los waypoints seran recorridos en sentido inverso al llegar al ultimo en la lista.")]
        PingPong, 
        [Tooltip("Los waypoints seran recorridos una sola vez.")]
        Once
    }
}