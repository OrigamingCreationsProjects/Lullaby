using TMPro;
using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Face Changer")]
    public class FaceChanger : MonoBehaviour
    {
        public SkinnedMeshRenderer faceRenderer;
        public Renderer eyesRenderer;
        public Color[] regularEyesColors;
        public Color[] HDREyesColors;
        public void ChangeFobosExpression(FobosEmotion emotion)
        {
            if (emotion == FobosEmotion.Angry)
            {
                eyesRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0 * .33f, 0));
                eyesRenderer.material.SetColor("_EmissionColor", HDREyesColors[0] * 3);
                eyesRenderer.material.SetColor("_BaseColor", regularEyesColors[0]);
            }
            if(emotion == FobosEmotion.Surprised)
                eyesRenderer.material.SetTextureOffset("_BaseMap", new Vector2(1 * .33f, 0));
            if (emotion == FobosEmotion.Normal)
            {
                eyesRenderer.material.SetTextureOffset("_BaseMap", new Vector2(2 * .33f, 0));
                eyesRenderer.material.SetColor("_EmissionColor", HDREyesColors[1]);
                eyesRenderer.material.SetColor("_BaseColor", regularEyesColors[1]);
            }
            
        }
    }
}