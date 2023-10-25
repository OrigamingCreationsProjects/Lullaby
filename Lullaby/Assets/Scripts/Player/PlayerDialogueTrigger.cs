using Cinemachine;
using Lullaby.Entities.NPC;
using Lullaby.Systems.DialogueSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lullaby.Entities
{
    public class PlayerDialogueTrigger: MonoBehaviour
    {
        private DialogueInterfaceManager _uiManager;
        private Talker _currentTalker;
        private Player _player;
        public CinemachineTargetGroup targetGroup;
        
        [Space]
        
        [Header("Post Processing")]
        public Volume dialogueDof;

        void Start()
        {
            _uiManager = DialogueInterfaceManager.Instance;
            _player = GetComponent<Player>();
            targetGroup = FindObjectOfType<CinemachineTargetGroup>();
        }
        
        void Update()
        {
            if (_player.SphereCast(transform.forward, 5f, out var sphereHit))
            {
                if (sphereHit.transform.gameObject.CompareTag("Talker"))
                {
                    Debug.Log("George asignado");
                    _currentTalker = sphereHit.transform.gameObject.GetComponent<Talker>();
                    _uiManager.currentNPC = sphereHit.transform.gameObject.GetComponent<NPCDialogueScript>();
                }
                else
                {
                    _currentTalker = null;
                    _uiManager.currentNPC = sphereHit.transform.gameObject.GetComponent<NPCDialogueScript>();
                }
            }
            if (_player.inputs.GetInteractDown() && !_uiManager.inDialogue && _currentTalker != null)
            {
                Debug.Log("Hablando con " + _currentTalker.name);
                targetGroup.m_Targets[1].target = _currentTalker.transform;
                //Funciones del UIManager
                _uiManager.SetCharNameAndColor();
                _uiManager.inDialogue = true;
                _uiManager.CameraChange(true);
                _uiManager.ClearText();
                _uiManager.FadeUI(true, .2f, .65f);
                //_currentTalker.FaceDirectionSmooth(transform.position, 800);
            }

            if (_player.inputs.GetInteractDown() && _uiManager.inDialogue)
            {
                _uiManager.NextDialogue();
            }
        }
    }
}