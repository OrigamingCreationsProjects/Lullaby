using DG.Tweening;
using Lullaby.Systems.DialogueSystem;
using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Dialogue Player State")]
    public class DialoguePlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
           DialogueInterfaceManager.Instance.SetCharNameAndColor();
           DialogueInterfaceManager.Instance.inDialogue = true;
           DialogueInterfaceManager.Instance.CameraChange(true);
           DialogueInterfaceManager.Instance.ClearText();
           DialogueInterfaceManager.Instance.FadeUI(true, .2f, .65f);
           DialogueInterfaceManager.Instance.MoveTalkerMouth();
           Sequence s = DOTween.Sequence();
           s.AppendCallback(() => player.inputs.enabled = false);
           s.AppendInterval(1f);
           s.AppendCallback(() => player.inputs.enabled = true);
           
        }

        protected override void OnExit(Player player)
        {
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => player.inputs.enabled = false);
            s.AppendInterval(0.3f);
            s.AppendCallback(() => player.inputs.enabled = true);
        }

        public override void OnStep(Player player)
        {
            if (player.inputs.GetInteractDown() && DialogueInterfaceManager.Instance.inDialogue)
            {
                DialogueInterfaceManager.Instance.NextDialogue();
            }

            if (!DialogueInterfaceManager.Instance.inDialogue)
            {
                player.states.Change<IdlePlayerState>();
            }

        }

        public override void OnContact(Player player, Collider other) { }
    }
}