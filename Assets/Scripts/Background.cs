using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class Background : MonoBehaviour
{
    public MusicInfo MusicInfoState;
    public SpriteRenderer fadeLayer;

    private SpriteRenderer sprite;
    private Color startColor;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        startColor = sprite.color;
    }

    public void PrepareLevelTransition(LevelParams level)
    {
        if (fadeLayer.color.a == 0) FadeOut(0.2f);
        MusicInfoState.DoOnNextBar(() => StartCoroutine(LevelTransition(level.color)));
    }

    public void EndLevel()
    {
        StartCoroutine(FadeOutIn());
    }

    public void ResetColor()
    {
        SetColor(startColor);
    }

    IEnumerator LevelTransition(Color nextColor)
    {
        TweenFactory.RemoveTweenKey("BgFadeOut", TweenStopBehavior.Complete);
        if (sprite.color != nextColor)
        {
            yield return SetColor(nextColor);
        }
        FadeIn(0.8f);
    }

    IEnumerator FadeOutIn()
    {
        FadeOut(0.8f);
        yield return new WaitForSeconds(0.8f);
        MusicInfoState.DoOnNextBar(() => FadeIn(0.6f));
    }

    WaitForSeconds SetColor(Color nextColor)
    {
        sprite.color = nextColor;
        fadeLayer.gameObject.Tween("ForegroundColorTween", fadeLayer.color, nextColor, 0.6f, TweenScaleFunctions.SineEaseInOut, (t) =>
        {
            fadeLayer.color = t.CurrentValue;
        });
        return new WaitForSeconds(0.6f);
    }


    void FadeIn(float duration)
    {
        TweenFactory.RemoveTweenKey("BgFadeOut", TweenStopBehavior.Complete);
        var transparent = fadeLayer.color;
        transparent.a = 0f;
        fadeLayer.gameObject.Tween("BgFadeIn", fadeLayer.color, transparent, duration, TweenScaleFunctions.SineEaseOut, (t) =>
        {
            fadeLayer.color = t.CurrentValue;
        });
    }

    void FadeOut(float duration)
    {
        TweenFactory.RemoveTweenKey("BgFadeIn", TweenStopBehavior.DoNotModify);
        fadeLayer.gameObject.Tween("BgFadeOut", fadeLayer.color, sprite.color, duration, TweenScaleFunctions.SineEaseOut, (t) =>
        {
            fadeLayer.color = t.CurrentValue;
        });
    }
}
