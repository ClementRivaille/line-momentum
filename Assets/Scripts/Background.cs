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
    private bool transitioning = false;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        startColor = sprite.color;
    }

    public void PrepareLevelTransition(LevelParams level)
    {
        MusicInfoState.DoOnNextBar(() => StartCoroutine(LevelTransition(level.color)));
    }

    public void EndLevel()
    {
        StartCoroutine(FadeOutIn());
    }

    public void PrepareCredits()
    {
        MusicInfoState.DoOnNextBar(() => SetColor(startColor));
    }

    public void CloseLevel()
    {
        if (transitioning) return;
        FadeOut(0.2f);
    }

    IEnumerator LevelTransition(Color levelColor)
    {

        transitioning = true;
        TweenFactory.RemoveTweenKey("BgFadeOut", TweenStopBehavior.Complete);
        fadeLayer.color = new Color(fadeLayer.color.r, fadeLayer.color.g, fadeLayer.color.b, 1.0f);
        if (sprite.color != levelColor)
        {
            yield return SetColor(levelColor);
        }
        FadeIn(0.8f);
        transitioning = false;
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
