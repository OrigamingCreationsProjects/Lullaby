using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Waypoints/Waypoint Manager")]
    public class WaypointManager : MonoBehaviour
    {
        public WaypointMode mode; // De que manera se recorren los waypoints
        public float waitTime; // Tiempo de espera entre waypoints
        public List<Transform> waypoints; // Lista de waypoints a recorrer

        protected Transform _current; // Waypoint actual

        protected bool _pong; // Indica si estamos yendo hacia delante o hacia atras en el modo PingPong
        protected bool _changing; // Indica si se esta cambiando de estado
        public Transform current
        {
            get
            {
                if (!_current)
                {
                    _current = waypoints[0];
                }

                return _current;
            }
            
            protected set { _current = value; }
        }
        
        public int index => waypoints.IndexOf(current); // Indice del waypoint actual

        public virtual void Next()
        {
            // Si ya esta cambiando a un estado nuevo, no hacemos nada
            if(_changing) return;
            
            //Necesitamos saber que modo se esta usando para recorrer los waypoints para dar una funcionalidad u otra.
            // Si el modo es PingPong, cambiamos de direccion cuando llegamos al final
            if (mode == WaypointMode.PingPong)
            {
                //Necesitamos la variable _pong para saber si estamos yendo hacia delante o hacia atras
                if (!_pong)
                {
                    _pong = (index + 1 == waypoints.Count); // Si estamos en el ultimo waypoint, cambiamos de direccion
                }
                else
                {
                    _pong = (index - 1 >= 0); // Si estamos en el primer waypoint, cambiamos de direccion
                }
                var next = !_pong? index + 1: index - 1; // Si no estamos en el ultimo waypoint, vamos al siguiente, si no, al anterior
                StartCoroutine(Change(next));
            }
            else if (mode == WaypointMode.Loop)
            {
                if (index + 1 < waypoints.Count)
                {
                    StartCoroutine(Change(index + 1)); // Si no estamos en el ultimo waypoint, vamos al siguiente
                }
                else
                {
                    StartCoroutine(Change(0)); // Si estamos en el ultimo waypoint, volvemos al primero
                }
            }
            else if (mode == WaypointMode.Once)
            {
                if(index + 1 < waypoints.Count)
                {
                    StartCoroutine(Change(index + 1)); // Si no estamos en el ultimo waypoint, vamos al siguiente
                }
            }
           
        }
        
        //Habría que hacer pruebas de rendimiento para ver si es mejor hacer esto con DoTween o con Corutinas
        protected virtual IEnumerator Change(int to)
        {
            _changing = true;
            yield return new WaitForSeconds(waitTime);
            current = waypoints[to];
            _changing = false;
        }
    }
}