using System;
using DG.Tweening;
using Lullaby.Entities.NPC;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Lullaby.Systems.DialogueSystem
{
    public class DialogueInterfaceManager : MonoBehaviour
    {
        public bool inDialogue = false;
        public static DialogueInterfaceManager Instance { get; private set; }

        public CanvasGroup canvasGroup;
        public TMP_Animated animatedText;
        public Image nameBubble;
        public TextMeshProUGUI nameTMP;
        
        [HideInInspector]
        public NPCDialogueScript currentNPC;
        
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
            animatedText.onDialogueFinish.AddListener(() => FinishDialogue());
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) && inDialogue)
            {
                if (canExit)
                {
                    CameraChange(false);
                    FadeUI(false, .2f, 0);
                    Sequence s = DOTween.Sequence();
                    s.AppendInterval(.8f);
                    s.AppendCallback(() => ResetState());
                }
                else if (nextDialogue)
                {
                    FadeUI(false, .2f, 0);
                    Sequence s = DOTween.Sequence();
                    s.AppendInterval(.8f);
                    s.AppendCallback(() => animatedText.ReadText(currentNPC.dialogueText.conversationBlock[dialogueIndex]));
                }
            }
        }

        public void NextDialogue()
        {
            if (canExit)
            {
                CameraChange(false);
                FadeUI(false, .2f, 0);
                Sequence s = DOTween.Sequence();
                s.AppendInterval(.8f);
                s.AppendCallback(() => ResetState());
            }

            if (nextDialogue)
            {
                animatedText.ReadText(currentNPC.dialogueText.conversationBlock[dialogueIndex]);
            }
        }

        public void FadeUI(bool show, float time, float delay)
        {
            Sequence fadeSequence = DOTween.Sequence();
            fadeSequence.AppendInterval(delay);
            fadeSequence.Append(canvasGroup.DOFade(show ? 1 : 0, time));
            if (show)
            {
                dialogueIndex = 0;
                fadeSequence.Join(canvasGroup.transform.DOScale(0, time * 2).From().SetEase(Ease.OutBack));
                //fadeSequence.AppendCallback(() => animatedText.text = currentNPC.dialogueText.conversationBlock[dialogueIndex]);
                fadeSequence.AppendCallback(() => animatedText.ReadText(currentNPC.dialogueText.conversationBlock[dialogueIndex]));
                
            }
        }

        public void SetCharNameAndColor()
        {
            nameTMP.text = currentNPC.data.NPCName;
            nameTMP.color = currentNPC.data.NPCNameColor;
            nameBubble.color = currentNPC.data.NPCColor;
        }
        
        public void CameraChange(bool dialogue)
        {
            gameCam.SetActive(!dialogue);
            dialogueCam.SetActive(dialogue);
            
            //Depth of field modifier
            float dofWeight = dialogueCam.activeSelf ? 1 : 0;
            //Crear DoVirtualFloat Para la mierda del DOF
        }

        public void DialogueDOF(float x)
        {
            //dialogueDof.weight = x;
        }
        
        public void ClearText()
        {
            animatedText.text = string.Empty;
        }

        public void ResetState()
        {
            //Reseteamos el NPC
            inDialogue = false;
            canExit = false;
        }
        public void FinishDialogue()
        {
            if (dialogueIndex < currentNPC.dialogueText.conversationBlock.Count - 1) 
            {
                dialogueIndex++;
                nextDialogue = true;
            }
            else
            {
                //Ponemos que no hay un dialogo despues
                nextDialogue = false;
                canExit = true;
            }
        }
        
        public void StartDialogue(NPCDialogueData npcDialogueData)
        {
            //dialogueInterface.StartDialogue(npcDialogueData);
        }

    }
}
