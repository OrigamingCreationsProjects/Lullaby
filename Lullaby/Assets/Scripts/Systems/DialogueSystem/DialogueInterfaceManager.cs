using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Lullaby.Systems.DialogueSystem
{
    public class DialogueInterfaceManager : MonoBehaviour
    {
        public static DialogueInterfaceManager Instance { get; private set; }

        public CanvasGroup canvasGroup;
        public TextMeshPro animatedText;
        public Image nameBubble;
        public TextMeshProUGUI nameTMP;
        
        [HideInInspector]
        //public NPCDialogueScript currentNPCDialogueScript;
        
        private int dialogueIndex;
        public bool canExit;
        public bool nextDialogue;

        [Space] 
        [Header("Cameras")] 
        public GameObject gameCam;
        public GameObject dialogueCam;
        
        //[Space]
        //public Volume dialogueDof;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            //Aqui suscribiremos el evento de que se ha terminado el dialogo
        }

        private void Update()
        {
            
        }

        public void StartDialogue(NPCDialogueData npcDialogueData)
        {
            //dialogueInterface.StartDialogue(npcDialogueData);
        }

        public void EndDialogue()
        {
            //dialogueInterface.EndDialogue();
        }
    }
}