using System;
using Cinemachine;
using DG.Tweening;
using Lullaby.Entities;
using Lullaby.Entities.NPC;
using Lullaby.Entities.States;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

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
        
        // -- Talker variables --
        public int currentTalkerIndex = 0; 
        
        [HideInInspector]
        public NPCDialogueScript currentNPC;
        public Talker currentTalker;
        public Animator currentTalkerAnimator;
        
        public bool canExit;
        public bool nextDialogue;
        public bool currentDialogueTextFinished;
        
        [Space] 
        
        [Header("Cameras")]  
        public GameObject gameCam;
        public GameObject dialogueCam;
        [Header("Zoom Variables")] 
        [Range(0, 40)] public float zoomValue = 25;
        [Range(0, 2)] public float zoomTime = 0.2f;
        [Range(0, 2)] public float zoomTransitionTime = 0.2f;

        protected int _talkHash;
        [Space]
        public Volume dialogueDof;

        private MMCinemachineZoom _zoom;
        
        private int dialogueIndex;

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
            _zoom = dialogueCam.GetComponent<MMCinemachineZoom>();
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
            string auxLine = currentNPC.dialogueText.conversationBlock[auxindex].dialogueLine.GetLocalizedString();
            string currentLine = currentNPC.dialogueText.conversationBlock[dialogueIndex].dialogueLine
                .GetLocalizedString();

            if ((animatedText.maxVisibleCharacters == auxLine.Length) 
                && nextDialogue)
            {
                nextDialogue = false;
                //CUIDADO CON ESTAS CUATRO LINEAS
                // FadeUI(false, .2f, 0);
                // Sequence s = DOTween.Sequence();
                // s.AppendInterval(.8f);
                // s.AppendCallback(() => animatedText.ReadText(currentNPC.dialogueText.conversationBlock[dialogueIndex]));
                if (CurrentTalkerHasChanged)
                { 
                    ChangeDialogueCameraTarget();
                    ChangeUITalker();
                }
                animatedText.ReadText(currentLine);
                currentTalkerAnimator.SetTrigger(_talkHash);
            } 
            else if(animatedText.maxVisibleCharacters != currentLine.Length)
            {
                Sequence s = DOTween.Sequence();
                s.AppendCallback(() => animatedText.StopAllCoroutines());
                s.AppendCallback(() => animatedText.maxVisibleCharacters = currentLine.Length);
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
                currentTalkerAnimator = currentTalker.GetComponentInChildren<Animator>();
                _talkHash = Animator.StringToHash("Talk");
                currentTalkerAnimator.SetTrigger(_talkHash);
                dialogueIndex = 0;
                fadeSequence.Join(canvasGroup.transform.DOScale(0, time * 2).From().SetEase(Ease.OutBack));
                //fadeSequence.AppendCallback(() => animatedText.text = currentNPC.dialogueText.conversationBlock[dialogueIndex]);
                fadeSequence.AppendCallback(() => animatedText.ReadText(currentNPC.dialogueText
                    .conversationBlock[dialogueIndex].dialogueLine.GetLocalizedString()));
            }
        }

        public void SetCharNameAndColor()
        {
            nameTMP.text = currentNPC.data.NPCName;
            nameTMP.color = currentNPC.data.NPCNameColor;
            nameBubble.color = currentNPC.data.NPCColor;
        } 
        private void ChangeCharNameAndColor(NPCDialogueData data)
        {
            nameTMP.text = data.NPCName;
            nameTMP.color = data.NPCNameColor;
            nameBubble.color = data.NPCColor;
        }
        
        public void CameraChange(bool dialogue)
        {
            gameCam.SetActive(!dialogue);
            dialogueCam.SetActive(dialogue);
            
            //Depth of field modifier
            float dofWeight = dialogueCam.activeSelf ? 1 : 0;
            DOVirtual.Float(dialogueDof.weight, dofWeight, .8f, DialogueDOF);
            //Crear DoVirtualFloat Para la mierda del DOF
        }

        public void DialogueDOF(float x)
        {
            dialogueDof.weight = x;
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
            currentTalkerIndex = 0;
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

        private void ChangeDialogueCameraTarget()
        {
            currentTalkerIndex = currentNPC.dialogueText.conversationBlock[dialogueIndex].actorId;
            _player.GetComponent<PlayerDialogueTrigger>().targetGroup.m_Targets[1].target = 
                currentTalker.talkersDialogueScripts[currentTalkerIndex].transform;
            if (currentNPC.dialogueText.conversationBlock[dialogueIndex].actorId == 1)
            {
                _zoom.Zoom(MMCameraZoomModes.Set, zoomValue, zoomTransitionTime, zoomTime, false);
            }
            else
            {
                _zoom.Zoom(MMCameraZoomModes.Set, 40, zoomTransitionTime, zoomTime, false);
            }

        }

        private void ChangeUITalker()
        {
            ChangeCharNameAndColor(currentTalker.talkersDialogueScripts[currentTalkerIndex].data);
        }

        private bool CurrentTalkerHasChanged =>
            currentTalkerIndex != currentNPC.dialogueText.conversationBlock[dialogueIndex].actorId;
        
    }
}
