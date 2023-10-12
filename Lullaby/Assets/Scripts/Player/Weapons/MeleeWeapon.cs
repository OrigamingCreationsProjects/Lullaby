using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    #region PARTE TEMPORAL HASTA TENER ANIMACIONES

    public Transform startPoint;  // Transform del punto de inicio (derecha del jugador)
    public Transform targetPoint; // Transform del punto de destino (izquierda del jugador)
    public AnimationCurve customAnimationCurve;
    #endregion
    
    public int damage = 10;

    [Serializable]
    public class AttackPoint
    {
        public float radius;
        public Vector3 offset;
        public Transform attackRoot;
#if UNITY_EDITOR
        //editor only as it's only used in editor to display the path of the attack that is used by the raycast
        [NonSerialized] public List<Vector3> previousPositions = new List<Vector3>();
#endif
    }
    
    //FALTAN Particulas

    public LayerMask targetLayers;
    public AttackPoint[] attackPoints = new AttackPoint[0];
    
    //FALTA Audio
    
    public bool throwingHit {get{return isThrowingHit;} set{isThrowingHit = value;}}
    
    protected GameObject owner; //GameObject del jugador
    
    protected Vector3[] previousPos = null; //Posiciones anteriores
    protected Vector3 direction; //Direccion del ataque
    
    protected bool isThrowingHit = false; //Si esta lanzando un ataque
    protected bool inAttack = false; //Si esta en ataque
    
    //FALTA Particulas pool
    
    protected static RaycastHit[] raycastHitCache = new RaycastHit[32]; // Lo usamos en el spherecast para no crear raycast cada vez que se use lo cual es más costoso
    protected static Collider[] colliderCache = new Collider[32]; 
    
    
    public void SetOwner(GameObject owner) //Establecemos quien lleva el arma para que pueda seguir el movimiento etc.
    {
        this.owner = owner;
    }

    public void BeginAttack(bool throwingAttack) 
    {
        //FALTA comprobar que no se esté reproduciendo el clip de ataque y entonces reproducir

        throwingHit = throwingAttack;

        inAttack = true;

        previousPos = new Vector3[attackPoints.Length]; // Inicializamos el array de posiciones anteriores
        for (int i = 0; i < attackPoints.Length; i++)
        {
            //Calculamos la posicion del ataque en coordenadas del mundo sumando el offset en coordenadas del mundo
            Vector3 worldPos = attackPoints[i].attackRoot.position +
                               attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
            previousPos[i] = worldPos; //Guardamos la posicion en el array de posiciones anteriores 
            
#if UNITY_EDITOR
            attackPoints[i].previousPositions.Clear(); //Limpiamos la lista de posiciones anteriores
            attackPoints[i].previousPositions.Add(previousPos[i]); //Añadimos la posicion al array de posiciones anteriores
#endif
        }
    }
    
    public void EndAttack()
    {
        inAttack = false;


        transform.position = targetPoint.position;
#if UNITY_EDITOR
        for (int i = 0; i < attackPoints.Length; i++)
        {
            attackPoints[i].previousPositions.Clear();
        }
#endif
    }
    
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            BeginAttack(true);
            Debug.Log("Ataque");
            //PARTE A ELIMINAR CUANDO MOVAMOS POR ANIMACIÓN
            transform.position = startPoint.position;

            // Utiliza DoTween para animar el movimiento desde puntoInicio a puntoDestino en una trayectoria curva.
            transform.DOMove(targetPoint.position, 0.5f) // Duración de la animación (en segundos)
                .SetEase(Ease.InOutQuad) // Curva de animación (puedes ajustarla)
                .OnComplete(AnimationComplete); // Opcional: llama a AnimationComplete cuando la animación se completa
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            EndAttack();
        }
       
        if (inAttack)
        {
            for (int i = 0; i < attackPoints.Length; i++)
            {
                AttackPoint pts = attackPoints[i];
                
                Vector3  worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset); //Calculamos la posicion del ataque en coordenadas del mundo sumando el offset en coordenadas del mundo
                Vector3 attackVector = worldPos - previousPos[i]; //Calculamos el vector de desplazamiento entre la posicion anterior y la actual

                if (attackVector.magnitude < 0.001f)
                {
                    // A zero vector for the sphere cast don't yield any result, even if a collider overlap the "sphere" created by radius. 
                    // so we set a very tiny microscopic forward cast to be sure it will catch anything overlaping that "stationary" sphere cast
                    attackVector = Vector3.forward * 0.0001f;
                }
                
                Ray r = new Ray(worldPos, attackVector.normalized); //Creamos un rayo con origen en la posicion del ataque y
                                                                    //direccion el vector de desplazamiento normalizado, es decir, la direccion del ataque
                
                int contacts = Physics.SphereCastNonAlloc(r, pts.radius, raycastHitCache, attackVector.magnitude, 
                    ~0, QueryTriggerInteraction.Ignore); //Lanzamos un rayo esferico con el radio del ataque y la longitud del vector de desplazamiento
                //~0 es para que tenga en cuenta todas las capas

                for (int k = 0; k < contacts; ++k)
                {
                    Collider col = raycastHitCache[k].collider; //Cogemos el collider del objeto con el que ha colisionado el rayo esferico

                    if (col != null)
                        CheckDamage(col, pts); //Comprobamos si el collider tiene el componente IDamageable y si lo tiene le hacemos daño
                }
                
                previousPos[i] = worldPos; //Guardamos la posicion en el array de posiciones anteriores
                
#if UNITY_EDITOR
                 pts.previousPositions.Add(previousPos[i]);
#endif
            }
        }
    }

    private bool CheckDamage(Collider other, AttackPoint pts)
    {
        return true;
    }
    
    void AnimationComplete()
    {
        // Este método se ejecutará cuando la animación esté completa.
        // Puedes realizar acciones adicionales aquí, como reiniciar el movimiento o desactivar el arma.
    }
    
#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                AttackPoint pts = attackPoints[i];

                if (pts.attackRoot != null)
                {
                    Vector3 worldPos = pts.attackRoot.TransformVector(pts.offset);
                    Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                    Gizmos.DrawSphere(pts.attackRoot.position + worldPos, pts.radius);
                }

                if (pts.previousPositions.Count > 1)
                {
                    UnityEditor.Handles.DrawAAPolyLine(10, pts.previousPositions.ToArray());
                }
            }
        }

#endif
}
