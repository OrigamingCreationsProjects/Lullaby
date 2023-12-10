using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lullaby;
using TMPro;
using UnityEngine;

public class GameEnderTemporally : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void EndGame()
    {
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => Fader.instance.FadeOut(PostFadeSequence));
        s.Append(text.DOFade(1.0f, 1.0f));
    }
    public void PostFadeSequence()
    {
        Sequence s = DOTween.Sequence();
        s.AppendInterval(2.0f);
        s.AppendCallback(() => Fader.instance.SetAlpha(0));
        s.AppendCallback(() => text.alpha = 0);
        s.AppendCallback(() => GameManager.LockCursor(false));
        s.AppendCallback(() => GameSceneLoader.instance.Load("CreditsSelection"));
    }
}
