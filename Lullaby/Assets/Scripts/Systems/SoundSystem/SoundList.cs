using UnityEngine;

namespace Systems.SoundSystem
{
    [CreateAssetMenu(fileName = "SoundList", menuName = "SoundManager/SoundList", order = 1)]
    public class SoundList : ScriptableObject
    {
        public Sound[] sounds;
    }
}