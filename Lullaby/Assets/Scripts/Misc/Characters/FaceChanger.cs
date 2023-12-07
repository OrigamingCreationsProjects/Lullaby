using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Face Changer")]
    public class FaceChanger : MonoBehaviour
    {
        public SkinnedMeshRenderer faceRenderer;
        public Renderer eyesRenderer;
        
        
        public void ChangeFace(int faceIndex)
        {
            eyesRenderer.material.SetTextureOffset("_BaseMap", new Vector2(faceIndex * .33f, 0));
        }
    }
}