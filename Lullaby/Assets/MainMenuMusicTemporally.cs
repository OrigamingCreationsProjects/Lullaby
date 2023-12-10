using System.Collections;
using System.Collections.Generic;
using Systems.SoundSystem;
using UnityEngine;

public class MainMenuMusicTemporally : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.PlayRandomPlaylistSong(MusicType.MainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
