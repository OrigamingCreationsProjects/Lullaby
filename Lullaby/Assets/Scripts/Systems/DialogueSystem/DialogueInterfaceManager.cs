using System;
using DG.Tweening;
using Lullaby.Entities;
using Lullaby.Entities.NPC;
using Lullaby.Entities.States;
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
        public bool currentDialogueTextFinished;
        
        [Space] 
        
        [Header("Cameras")] 
        public GameObject gameCam;
        public GameObject dialogueCam;
        
        //[Space]
        //public Volume dialogueDof;


        private Player _player;
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
            _player = FindObjectOfType<Player>();
            canvasGroup.interactable = false;
        }

        private void Update()
        {
            // if(Input.GetKeyDown(KeyCode.Space) && inDialogue)
            // {
            //     if (canExit)
            //     {
            //         CameraChange(false);
            //         FadeUI(false, .2f, 0);
            //         Sequence s = DOTween.Sequence();
            //         s.AppendInterval(.8f);
            //         s.AppendCallback(() => ResetState());
            //     }
            //     else if (nextDialogue)
            //     {
            //         FadeUI(false, .2f, 0);
            //         Sequence s = DOTween.Sequence();
            //         s.AppendInterval(.8f);
            //         s.AppendCallback(() => animatedText.ReadText(currentNPC.dialogueText.conversationBlock[dialogueIndex]));
            //     }
            // }
            
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
                Debug.Log("Salimos del dialogo");
            }
            int auxindex = dialogueIndex == 0? dialogueIndex : dialogueIndex - 1;
            if ((animatedText.maxVisibleCharacters == currentNPC.dialogueText.conversationBlock[auxindex].Length) 
                && nextDialogue)
            {
                nextDialogue = false;
                //CUIDADO CON ESTAS CUATRO LINEAS
                // FadeUI(false, .2f, 0);
                // Sequence s = DOTween.Sequence();
                // s.AppendInterval(.8f);
                // s.AppendCallback(() => animatedText.ReadText(currentNPC.dialogueText.conversationBlock[dialogueIndex]));
                animatedText.ReadText(currentNPC.dialogueText.conversationBlock[dialogueIndex]);
            } 
            else if(animatedText.maxVisibleCharacters != currentNPC.dialogueText.conversationBlock[dialogueIndex].Length)
            {
                Sequence s = DOTween.Sequence();
                s.AppendCallback(() => animatedText.StopAllCoroutines());
                s.AppendCallback(() => animatedText.maxVisibleCharacters = currentNPC.dialogueText.conversationBlock[dialogueIndex].Length);
                s.AppendInterval(0.5f);
                s.AppendCallback(() => animatedText.onDialogueFinish?.Invoke());
                animatedText.StopAllCoroutines();
                //CUIDADO CON ESTA LINEA YA QUE PUEDE HACER QUE LOS SPLITS SE VEAN. OSEA LAS TAGS
                //animatedText.maxVisibleCharacters = currentNPC.dialogueText.conversationBlock[dialogueIndex].Length;
                //animatedText.onDialogueFinish?.Invoke();
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
            NotifyDialogueFinished();
        }
        public void FinishDialogue()
        {
            currentDialogueTextFinished = true;
            if (dialogueIndex < currentNPC.dialogueText.conversationBlock.Count - 1) 
            {
                dialogueIndex++;
                nextDialogue = true;
                Debug.Log($"INDICE DE DIALOGO ACTUAL {dialogueIndex}");
            }
            else
            {
                //Ponemos que no hay un dialogo despues
                nextDialogue = false;
                canExit = true;
            }
        }
        public void NotifyDialogueStarted()
        {
            currentNPC.gameObject.GetComponent<Talker>().talkerEvents.OnDialogueStarted?.Invoke();
        }

        public void NotifyDialogueFinished()
        {
            currentNPC.gameObject.GetComponent<Talker>().talkerEvents.OnDialogueFinished?.Invoke();
        }
        
    }
}
