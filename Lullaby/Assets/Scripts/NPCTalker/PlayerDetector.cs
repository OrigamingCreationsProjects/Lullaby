using Lullaby.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lullaby.Entities.NPC
{
    public class PlayerDetector : MonoBehaviour
    {
        private Talker _talker;
        private ContextIndicator _contextIndicator;
        
        private void Start()
        {
            _talker = GetComponentInParent<Talker>();
            _contextIndicator = transform.parent.GetComponentInChildren<ContextIndicator>();
            Debug.Log("Talker: " + _talker);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Player detected");
            Debug.Log("Other tag: " + other.tag);
            if (other.CompareTag(GameTags.Player))
            {
                Debug.Log("Entered player collider");
                _talker.talkerEvents.OnPlayerDetected.Invoke();
                _contextIndicator.ShowContextIndicator();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(GameTags.Player))
            {
                _talker.talkerEvents.OnPlayerGone?.Invoke();
                _contextIndicator.HideContextIndicator();
            }
        }
    }
}