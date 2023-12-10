using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Lullaby;
using Systems.SoundSystem;
using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    public float scrollTime = 120f;
    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.currentSong.Stop();
        GetComponent<AudioSource>().Play();
        Scroll();
    }

    private void Scroll()
    {
        transform.DOLocalMoveY(6000, scrollTime).onComplete += () => EndCredits();
    }

    private void EndCredits()
    {
        GetComponent<AudioSource>().Stop();
        GameSceneLoader.instance.Load("MainMenu");
    }
}
