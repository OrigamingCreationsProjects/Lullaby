using System;
using Cinemachine;
using UnityEngine;

namespace Lullaby
{
    public class CameraTrigger : MonoBehaviour
    {
        private CinemachineBrain _cameraBrain;
        private Transform _camerasGroup;
        [Header("Camera Settings")]
        public bool activatesCamera = false;
        public CinemachineVirtualCamera camera;
        public bool cut;
        public CinemachineBlendDefinition.Style blendStyle;
        
        private void Start()
        {
            _camerasGroup = GameObject.Find("LauncherCameras").transform;
            _cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        }

        public void SetCamera()
        {
            _cameraBrain.m_DefaultBlend.m_Style = cut ? CinemachineBlendDefinition.Style.Cut : CinemachineBlendDefinition.Style.EaseOut;

            if (_camerasGroup.childCount <= 0)
            {
                return;
            }
            

            for (int i = 0; i < _camerasGroup.childCount; i++)
            {
                _camerasGroup.GetChild(i).gameObject.SetActive(false);
            }
            camera.gameObject.SetActive(activatesCamera);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, .1f);
        }
    }
}