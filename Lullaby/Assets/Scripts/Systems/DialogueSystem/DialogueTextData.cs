using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Pseudo;

namespace Lullaby.Systems.DialogueSystem
{
    [CreateAssetMenu(fileName = "NewDialogueTextData", menuName = "DialogueSystem/NPC/NewDialogueTextData", order = 0)]
    public class DialogueTextData : ScriptableObject
    {
        public List<DialogueLine> conversationBlock;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public int actorId;
        public LocalizedString dialogueLine;
    }
}