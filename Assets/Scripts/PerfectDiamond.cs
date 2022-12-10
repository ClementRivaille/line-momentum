using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PerfectDiamond : MonoBehaviour
{

    private CanvasGroup canva;
    private float opacity;
    public Image fillImage;

    public bool filled = false;
    public bool hidden = false;

    void Start()
    {
        canva = GetComponent<CanvasGroup>();
        opacity = canva.alpha;

        canva.alpha = hidden ? 0f : opacity;
        fillImage.color = filled ? Color.white : new Color(1f, 1f, 1f, 0f);
    }

    public void SetHidden(bool value)
    {
        if (value != hidden)
        {
            hidden = value;
            gameObject.Tween("hide_diamond", canva.alpha, value ? 0f : opacity,
                0.2f, TweenScaleFunctions.SineEaseInOut,
                (t) => { canva.alpha = t.CurrentValue;
            });
        }
    }

    public void SetFilled(bool value)
    {
        if (value != filled)
        {
            filled = value;
            gameObject.Tween("fill_diamond", fillImage.color, value ? Color.white : new Color(1f,1f,1f,0f),
                0.4f, TweenScaleFunctions.SineEaseInOut,
                (t) => {
                    fillImage.color = t.CurrentValue;
                });
        }
    }
}
