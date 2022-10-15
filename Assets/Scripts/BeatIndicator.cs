using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public SpriteRenderer FailBg;

    public BeatStatus status { get; private set; } = BeatStatus.Pending;

    private Animation animationManager;

    private void Start()
    {
        animationManager = GetComponent<Animation>();
    }

    public void Validate()
    {
        
        animationManager.Play("ValidateBeat");
        status = BeatStatus.Complete;

        // Ugly fix for when the reset animation didn't finish
        FailBg.color = new Color(0f, 0f, 0f, 0f);
    }

    public void Fail()
    {
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


    public void SetColors(Color backgroundColor, Color bordersColor)
    {
        GetComponent<SpriteRenderer>().color = bordersColor;
        Background.color = backgroundColor;
    }
}
