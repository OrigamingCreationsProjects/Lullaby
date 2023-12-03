using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
namespace Lullaby.Entities.States
{
    public class MoonFlyPlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
            player.velocity = Vector3.zero;
            player.moonLauncher.StartCenterLaunch();
        }

        protected override void OnExit(Player player)
        {
            player.transform.parent = player.initialParent;
            player.lateralVelocity = player.moonLauncher.moonPathCart.transform.forward;
        }

        public override void OnStep(Player player)
        {
            if (player.moonLauncher.flying)
            {
                //animator.SetFloat("Path", dollyCart.m_Position);
                player.moonLauncher.playerParent.transform.position = player.moonLauncher.moonPathCart.transform.position;
                if (!player.moonLauncher.almostFinished)
                {
                    player.moonLauncher.playerParent.transform.rotation = player.moonLauncher.moonPathCart.transform.rotation;
                }
            }

            if(player.moonLauncher.moonPathCart.m_Position > .7f && !player.moonLauncher.almostFinished && player.moonLauncher.flying)
            {
                player.moonLauncher.almostFinished = true;
                
                player.moonLauncher.playerParent.DORotate(new Vector3(360 + 180, 0, 0), .5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
                    .OnComplete(() => player.moonLauncher.playerParent.DORotate(
                        new Vector3(-90, player.moonLauncher.playerParent.eulerAngles.y, 
                            player.moonLauncher.playerParent.eulerAngles.z), .2f));
            }
        }

        public override void OnContact(Player player, Collider other) { }
    }
}