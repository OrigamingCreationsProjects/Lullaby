using System.Collections;
using System.Collections.Generic;
using Systems.SoundSystem;
using UnityEngine;

public class MainMenuMusicTemporally : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(MusicManager.instance.currentSong.name != "MainMenu_Music")
            MusicManager.instance.PlayRandomPlaylistSong(MusicType.MainMenu);
    }
    
}
