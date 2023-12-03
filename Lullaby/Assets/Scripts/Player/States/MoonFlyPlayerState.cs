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
        protected bool flying = false;
        protected bool almostFinished;
        protected float speed;
        protected float lastDashTime;
        
        protected override void OnEnter(Player player)
        {
            // player.StartCoroutine(CenterLaunch(player));
            player.velocity = Vector3.zero;
        }

        protected override void OnExit(Player player)
        {
            player.transform.parent = player.initialParent;
            player.lateralVelocity = player.moonPathCart.transform.forward;
        }

        public override void OnStep(Player player)
        {
            // if (flying)
            // {
            //     //animator float path
            //     player.transform.position = player.moonPathCart.transform.position;
            //     if (!almostFinished)
            //     {
            //         player.transform.rotation = player.moonPathCart.transform.rotation;
            //     }
            // }
            //
            // if(player.moonPathCart.m_Position > 0.7f && !almostFinished && flying)
            // {
            //     almostFinished = true;
            //     
            //     player.transform.DORotate(new Vector3(360 + 180, 0, 0), .5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
            //         .OnComplete(() => player.transform.DORotate(
            //             new Vector3(-90, player.transform.eulerAngles.y, player.transform.eulerAngles.z), .2f));
            // }
        }

        public override void OnContact(Player player, Collider other) { }


        IEnumerator CenterLaunch(Player player)
        {
            player.SetInputEnabled(false);
            DOTween.KillAll();
            
            // if(player.launchObject.GetComponent<CameraTrigger>() != null)
            //     player.launchObject.GetComponent<CameraTrigger>().SetCamera();
            
            player.moonPathCart.m_Position = 0;
            player.moonPathCart.m_Path = null;
            player.moonPathCart.m_Path = player.launchObject.GetComponent<CinemachineSmoothPath>();
            player.moonPathCart.enabled = true;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            
            Sequence CenterLaunch = DOTween.Sequence();
            CenterLaunch.Append(player.transform.DOMove(player.moonPathCart.transform.position, .2f));
            CenterLaunch.Join(player.transform.DORotate(player.moonPathCart.transform.eulerAngles 
                                                        + new Vector3(90, 0, 0), .2f));
            //Animation => CenterLaunch.Join(moonAnimation.Reset(.2f));
            CenterLaunch.OnComplete(() => LaunchSequence(player));
        }

        Sequence LaunchSequence(Player player)
        {
            CinemachineSmoothPath path = player.launchObject.GetComponent<CinemachineSmoothPath>();
            float finalSpeed = path.PathLength / (17); //CAMBIAR POR VARIABLE

            flying = true;
            //Animator flying

            Sequence s = DOTween.Sequence();

            s.Append(player.transform.DOMove(player.transform.localPosition - player.transform.up, 0.15f)); //CAMBIAR POR VARIABLE
            s.Join(player.transform.DOLocalRotate(new Vector3(0, 360 * 2, 0), 0.15f, 
                RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart));
            //s.Join(moonAnimation.PullMoon(0.15f)); //CAMBIAR POR VARIABLE
            s.AppendInterval(.5f); //CAMBIAR POR VARIABLE
            //s.AppendCallback(() => trail) //Que emita el trail
            //s.AppendCallback(() => followParticles.Play()); //Que emita las particulas
            s.Append(DOVirtual.Float(player.moonPathCart.m_Position, 1, finalSpeed, 
                    (x) => player.moonPathCart.m_Position = x)).SetEase(Ease.InCubic); //.SetEase(pathCurve);
            //s.Join(moonAnimation.PunchMoon(.5f));
            s.Join(player.transform.DOLocalMove(new Vector3(0, 0, -.5f), .5f)); //CAMBIAR POR VARIABLE
            s.Join(player.transform.DOLocalRotate(new Vector3(0, 360, 0), (finalSpeed / 1.3f), RotateMode.LocalAxisAdd))
                .SetEase(Ease.InOutSine);
            s.AppendCallback(() => Land(player)); //CAMBIAR POR VARIABLE

            return s;

        }

        protected void Land(Player player)
        {
            player.moonPathCart.enabled = false;
            player.moonPathCart.m_Position = 0;
            player.SetInputEnabled(true);
           
            flying = false;
            almostFinished = false;
            //animator fly = false

            //stop follow particles
            //stop trail
        }
        
        public void PathSpeed(float x)
        {
            
        }
    }
}