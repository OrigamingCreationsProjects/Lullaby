using UnityEngine;

namespace Lullaby.Systems.DialogueSystem
{
    [CreateAssetMenu(fileName = "NewNPCDialogueData", menuName = "DialogueSystem/NPC/NewNPCDialogueData", order = 0)]
    public class NPCDialogueData : ScriptableObject
    {
        public string NPCName;
        public Color NPCColor;
        public Color NPCNameColor;
        public DialogueTextData dialogue;
    }
}