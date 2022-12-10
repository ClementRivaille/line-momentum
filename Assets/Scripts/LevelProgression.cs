using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class LevelProgression : MonoBehaviour
{
    public MusicInfo MusicInfoState;
    public GameObject DotPrefab;
    public int NbLoops;

    public Color InactiveColor;
    public Color ActiveColor;

    public PerfectDiamond diamond;

    private int currentProgress;
    private bool isPerfect = false;

    private List<SpriteRenderer> dots = new List<SpriteRenderer> ();

    private void Start()
    {
        MusicInfoState.onBar.AddListener(RefreshDots);
    }

    public void SetupDots()
    {
        if (dots.Count == 0)
        {
            var marginLeft = 1f / (float)(NbLoops * 2);
            for (int i = 0; i < NbLoops; i++)
            {
                var dot = Instantiate(DotPrefab, transform);
                var dotPosition = dot.GetComponent<RectTransform>();
                dotPosition.anchorMin = new Vector2(marginLeft + (float)i / (float)NbLoops, 0.5f);
                dotPosition.anchorMax = new Vector2(marginLeft + (float)i / (float)NbLoops, 0.5f);
                dotPosition.anchoredPosition = Vector2.zero;

                var dotSprite = dot.GetComponent<SpriteRenderer>();
                dotSprite.color = InactiveColor;
                dots.Add(dotSprite);
            }

        } else
        {
            dots.ForEach(dot => dot.gameObject.SetActive(true));
            diamond.gameObject.SetActive(true);
        }
    }

    public void UpdateProgress(int value)
    {
        currentProgress = value;
    }

    public void UpdatePerfect(bool value)
    {
        isPerfect = value;
        if (!isPerfect)
        {
            diamond.SetHidden(true);
        }
    }

    private void RefreshDots() {
        for (int i = 0; i < dots.Count; i++)
        {
            var dot = dots[i];
            if (i < currentProgress && dot.color != ActiveColor) {
                dot.gameObject.Tween("TweenColor" + i, InactiveColor, ActiveColor, 0.3f, TweenScaleFunctions.SineEaseOut, (t) =>
                {
                    dot.color = t.CurrentValue;
                });
            }
            else if (i >= currentProgress && dot.color != InactiveColor)
            {
                dot.gameObject.Tween("TweenColor" + i, ActiveColor, InactiveColor, 0.4f, TweenScaleFunctions.SineEaseInOut, (t) =>
                {
                    dot.color = t.CurrentValue;
                });
            }
        }

        if (currentProgress == 0)
        {
            diamond.SetFilled(false);
            diamond.SetHidden(false);
        }
        else if (currentProgress == NbLoops && isPerfect)
        {
            diamond.SetFilled(true);
        }
    }

    public void ClearDots()
    {
        dots.ForEach(dot => {
            dot.color = InactiveColor;
            dot.gameObject.SetActive(false);
        });
        diamond.gameObject.SetActive(false);
    }
}
