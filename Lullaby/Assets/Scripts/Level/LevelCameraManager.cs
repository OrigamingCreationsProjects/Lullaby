using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Lullaby.LevelManagement
{
    [AddComponentMenu("Lullaby/Level/Level Camera Manager")]
    public class LevelCameraManager : Singleton<LevelCameraManager>
    {
        public CinemachineVirtualCamera playerCamera;
        public CinemachineVirtualCamera helpingCamera;
        public float helpingSequenceTime = 1f;
        
        private int activeCameraPrioryty = 10;
        private int inactiveCameraPriority = 0;

        public void ShowActionConsequence(Transform target)
        {
            //ChangeHelpingCameraTarget(target);
            //StartCoroutine(ShowActionRoutine());
            ShowActionTween(target);
        }
        
        private IEnumerator ShowActionRoutine()
        {
            yield return new WaitForSeconds(0.3f);
            ChangeActiveCamera(true);
            yield return new WaitForSeconds(helpingSequenceTime);
            ChangeActiveCamera(false);
        }
        
        private void ShowActionTween(Transform target)
        {
            //Debug.Log("ShowActionTween");
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => ChangeHelpingCameraTarget(target));
            //s.AppendInterval(0.5f); 
            s.AppendCallback(() => ChangeActiveCamera(true));
            s.AppendInterval(helpingSequenceTime);
            s.AppendCallback(() => ChangeActiveCamera(false));
        }
        
        private void ChangeActiveCamera(bool showHelpingCamera)
        {
            playerCamera.Priority = showHelpingCamera? inactiveCameraPriority : activeCameraPrioryty;
            helpingCamera.Priority = showHelpingCamera? activeCameraPrioryty : inactiveCameraPriority;
        }

        private void ChangeHelpingCameraTarget(Transform target)
        {
            helpingCamera.gameObject.SetActive(false);
            
            helpingCamera.LookAt = target;
            helpingCamera.Follow = target;

            helpingCamera.gameObject.SetActive(true);
        }
    }
}