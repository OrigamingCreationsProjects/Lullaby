using System;
using System.Collections;
using System.Collections.Generic;
using Lullaby;
using UnityEngine;

namespace Systems.SoundSystem
{
    [AddComponentMenu("Systems/Sound System/Music Changer")]
    public class MusicChanger : MonoBehaviour
    {
        public MusicType zoneMusicType;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameTags.Player) && zoneMusicType != MusicManager.instance.currentPlaylist.musicType)
            {
                Debug.Log("Se cambia playlist");
                MusicManager.instance.ChangeCurrentPlaylist(zoneMusicType);
            }
        }
    }
}
