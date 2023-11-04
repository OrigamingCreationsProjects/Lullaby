using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace  Lullaby.Entities
{
    [AddComponentMenu("Custom Movement/Entity/Entity Controller")]
    public class EntityController : MonoBehaviour
    {
        //Definimos el maximo de pendiente en grados de inclinación que podremos subir
        [Range(0, 180f)] 
        public float slopeLimit = 45f;
        //Desplazamiento de cada paso que el personaje realice
        [Min(0)] public float stepOffset = 0.3f;

        //Definimos un grosor de piel para tenerlo como separación y tener colisiones mas realistas
        [Min(0.0001f)] 
        public float skinWidth = 0.005f;
        //El centro de la entidad
        public Vector3 center;
        //Radio del collider
        [Min(0)] 
        public float radius = 0.5f;
        //Altura del controlador y por tanto collider
        [Min(0)] 
        public float height = 2f;

        public LayerMask collisionLayer = -5;
        
        //PROPIEDADES
        protected const int _maxCollisionSteps = 3;

        protected Rigidbody _characterRigidbody;
        protected Collider[] _overlaps = new Collider[128]; // Maximo de colisiones o solapamiento de colisiones a la vez

        public bool handleCollision { get; set; } = true;
        
        public new CapsuleCollider collider { get; protected set; }

        public Bounds bounds => collider.bounds; 
        
        //Calculamos el desplazamiento (o distancia) desde el centro de la capsula hasta el extremo más alejado (altura o anchura)
        //Esto nos sirve para colocar objetos relacionados con la capsula, posición de VFX o colisiones de manera precisa en funcion
        //de la forma de la capsula, ya que al estar usando un character controller algunas fisicas se ajustan manualmente
        public Vector3 capsuleOffset => transform.up * (Mathf.Max(radius * 2, height) * 0.5f - radius);

        protected void Awake()
        {
            InitializeCollider();
            InitializeRigidbody();
            RefreshCollider();
        }

        protected void OnEnable() => collider.enabled = true;
        protected void OnDisable() => collider.enabled = false;


        private void InitializeCollider()
        {
            collider = gameObject.AddComponent<CapsuleCollider>();
            collider.isTrigger = true;
        }
        private void InitializeRigidbody()
        {
            if (!TryGetComponent(out _characterRigidbody))
            {
                _characterRigidbody = gameObject.AddComponent<Rigidbody>();
            }

            _characterRigidbody.isKinematic = true;
        }

        public virtual void Resize(float height) //CAMBIO REALIZADO EN CALCULO DE CENTER
        {
            var delta = height - this.height; //Calculamos la diferencia de altura
            this.height = height; 
            center += Vector3.up * delta * 0.5f; //Ajustamos el centro de la capsula en funcion de la altura
            RefreshCollider();
        }
        private void RefreshCollider()
        {
            collider.radius = radius - skinWidth;
            collider.height = height - skinWidth;
            collider.center = center;
        }

        public virtual void Move(Vector3 motion)
        {
            if(!enabled) return;

            var position = transform.position;

            if (handleCollision)
            {
                //Con la siguiente linea pasamos de coordenadas del mundo a coordenadas locales
                var localMotion = transform.InverseTransformDirection(motion); 
                var lateralMotion = new Vector3(localMotion.x, 0, localMotion.z);
                var verticalMotion = new Vector3(0, localMotion.y, 0);
                
                lateralMotion = transform.TransformDirection(lateralMotion);
                verticalMotion = transform.TransformDirection(verticalMotion);

                position = MoveAndSlide(position, lateralMotion);
                position = MoveAndSlide(position, verticalMotion, true);
                position = HandlePenetration(position);
            }
            else
            {
                position += motion;
            }

            transform.position = position;
        }


        protected virtual Vector3 MoveAndSlide(Vector3 position, Vector3 motion, bool verticalPass = false)
        {
            for (int i = 0; i < _maxCollisionSteps; i++)
            {
                var moveDistance = motion.magnitude;
                var moveDirection = motion / moveDistance;
                
                if(moveDistance <= 0.001f) break;
                
                var distance = moveDistance + skinWidth + radius;
                var origin = position + transform.rotation * center - moveDirection * radius;
                var point1 = origin + capsuleOffset;
                var point2 = origin - capsuleOffset;
                
                if(!verticalPass)
                    point2 += transform.up * stepOffset;

                if (Physics.CapsuleCast(point1, point2, radius, moveDirection, out var hit,
                        distance, collisionLayer, QueryTriggerInteraction.Ignore))
                {
                    var safeDistance = hit.distance - skinWidth - radius;
                    var offset = moveDirection * safeDistance;
                    var leftOver = motion - offset; //Lo que nos queda de movimiento
                    var angle = Vector3.Angle(transform.up, hit.normal); //Angulo de la normal con respecto a la vertical

                    position += offset;
                    
                    if (angle <= slopeLimit && verticalPass) continue;
                    
                    motion = Vector3.ProjectOnPlane(leftOver, hit.normal);
                }
                else
                {
                    position += motion;
                    break;
                }
            }

            return position;
        }
        protected virtual Vector3 HandlePenetration(Vector3 position)
        {
            var origin = position + transform.rotation * center; //Centro de la capsula
            var point1 = origin + capsuleOffset; //Punto superior de la capsula
            var point2 = origin - capsuleOffset; //Punto inferior de la capsula
            var penetrations = Physics.OverlapCapsuleNonAlloc(point1, point2, radius, 
                _overlaps, collisionLayer, QueryTriggerInteraction.Ignore); //Obtenemos todas las colisiones en un array prefefinido

            for (int i = 0; i < penetrations; i++)
            {
                if(Physics.ComputePenetration(collider, transform.position, transform.rotation, 
                       _overlaps[i], _overlaps[i].transform.position, _overlaps[i].transform.rotation, 
                       out var direction, out var distance))
                {
                    if(_overlaps[i].transform == transform) continue;
                    if (_overlaps[i].CompareTag(GameTags.Platform)) //Si la colision es con una plataforma
                    {
                        position += transform.up * height * 0.5f; //Subimos la capsula a la mitad de su altura
                        continue;
                    }
                    position += direction * distance; //Movemos la capsula en la direccion y distancia de la colision
                }
            }

            return position;
        }
        //Conversion implicita de EntityController a Collider para que sea mas facil de usar y los tipos no den conflictos.
        //Tambien podremos usar EntityController como si fuera un Collider
        public static implicit operator Collider(EntityController controller) => controller.collider;

        protected void OnDrawGizmos()
        {
            var origin = transform.position + transform.rotation * center;
            var point1 = origin + capsuleOffset;
            var point2 = origin - capsuleOffset;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(point1, radius); 
            Gizmos.DrawWireSphere(point2, radius);
        }
    }
    
}
