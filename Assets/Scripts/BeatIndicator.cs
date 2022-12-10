using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum BeatStatus
{
    Pending,
    Complete,
    Failed
}

[RequireComponent(typeof(Animation), typeof(SpriteRenderer))]
public class BeatIndicator : MonoBehaviour
{
    public SpriteRenderer Background;

    public UnityEvent OnAnimationEnd = new UnityEvent();

    public BeatStatus status { get; private set; } = BeatStatus.Pending;

    private Animation animationManager;

    private void Start()
    {
        GetAnimator();
    }

    public void Validate()
    {
        GetAnimator();
        animationManager.Play("ValidateBeat");
        status = BeatStatus.Complete;
    }

    public void Fail()
    {
        GetAnimator();
        status = BeatStatus.Failed;
        animationManager.Play("MissBeat");
    }

    public void Reset()
    {
        if (status == BeatStatus.Complete)
        {
            animationManager.Play("ResetBeat");
        } else if (status == BeatStatus.Failed)
        {
            animationManager.Play("ResetMissBeat");
        }
        status = BeatStatus.Pending;
    }

    void GetAnimator() {
        if (!animationManager)
        {
            animationManager = GetComponent<Animation>();
        }
    }


    public void SetColors(Color backgroundColor, Color bordersColor)
    {
        GetComponent<SpriteRenderer>().color = bordersColor;
        Background.color = backgroundColor;
    }

    void TriggerAnimationEnd()
    {
        OnAnimationEnd.Invoke();
    }
}
