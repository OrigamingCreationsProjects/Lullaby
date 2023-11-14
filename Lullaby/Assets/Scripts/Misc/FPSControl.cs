using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/FPS Control")]
    public class FPSControl : MonoBehaviour
    {
        void Start()
        {
            Application.targetFrameRate = -1; //Screen.currentResolution.refreshRate;
        }
    }
}
