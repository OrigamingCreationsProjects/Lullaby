using Lullaby.Systems.DialogueSystem;
using TMPro;
using UnityEngine;

namespace Lullaby.Entities.NPC
{
    public class NPCDialogueScript : MonoBehaviour
    {
        public NPCDialogueData data;
        public DialogueTextData dialogueText;


        private TMP_Animated _animatedText;
        //Audio
        //Animaciones
        //EyesRenderer
        
        //Particulas
        
        private void Start()
        {
            _animatedText = DialogueInterfaceManager.Instance.animatedText;
        }
    }
}