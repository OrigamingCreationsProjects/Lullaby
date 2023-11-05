using DG.Tweening;
using UnityEngine;

namespace Lullaby
{
    public class Switch: MonoBehaviour
    {
        public Transform targetPos;
        public GameObject sphere;
        public float duration = 1f;
        public Color activateColor;
        
        public virtual void ActiveSwitch()
        {
            sphere.transform.DOMoveY(targetPos.position.y, duration);
            sphere.GetComponent<MeshRenderer>().material.color = activateColor; //.DOColor(activateColor, duration);
            sphere.GetComponent<Floater>().enabled = false;
        }
    }
}