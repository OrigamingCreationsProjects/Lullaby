using System.Collections.Generic;
using UnityEngine;

namespace Lullaby.Systems.DialogueSystem
{
    [CreateAssetMenu(fileName = "NewDialogueTextData", menuName = "DialogueSystem/NPC/NewDialogueTextData", order = 0)]
    public class DialogueTextData : ScriptableObject
    {
        [TextArea(4, 4)]
        public List<string> conversationBlock;
    }
}