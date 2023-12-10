using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    public float scrollTime = 120f;
    // Start is called before the first frame update
    void Start()
    {
        Scroll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Scroll()
    {
        transform.DOLocalMoveY(6000, scrollTime);
    }
}
