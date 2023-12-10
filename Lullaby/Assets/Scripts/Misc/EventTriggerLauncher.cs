using UnityEngine;
using UnityEngine.Events;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Lullaby/Misc/Event Trigger Launcher")]
    public class EventTriggerLauncher : MonoBehaviour
    {
        public UnityEvent onTriggerEnterEvent;
        public bool oneTimeTrigger = true;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameTags.Player))
            {
                onTriggerEnterEvent?.Invoke();
                if (oneTimeTrigger)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}