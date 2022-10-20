using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine;
using System;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MusicInfo", menuName = "ScriptableObjects/MusicInfo", order = 0)]
public class MusicInfo : ScriptableObject
{
    public float Precision = 0.06f;

    public float barPosition { get; private set; }
    public float currentTime { get; private set; }

    public float barDuration;
    public float beatDuration;

    
    public UnityEvent onBeat = new UnityEvent();
    public UnityEvent onBar = new UnityEvent();
    public UnityEvent onMusicStarted = new UnityEvent();

    private List<UnityAction> oneTimeBarListeners = new List<UnityAction>();

    public void Init()
    {
        barPosition = 0f;
        currentTime = 0f;
        onMusicStarted.Invoke();
        onBar.AddListener(BarActions);
    }

    public void updatePosition(float time)
    {
        barPosition = time / barDuration;

        currentTime = Math.Min(barDuration, Math.Max(0f, time));
    }

    public bool IsCloseTo(float time)
    {
        if (Math.Abs(time - currentTime) < Precision)
        {
            return true;
        }
        if (time< Precision)
        {
            return Math.Abs(barDuration - currentTime) < Precision;
        }
        else if (Math.Abs(barDuration - time) < Precision)
        {
            return currentTime < Precision;
        }
        return false;
    }

    public bool IsPassed(float time)
    {
        return Math.Abs(barDuration - currentTime) > Precision &&
            currentTime > time &&
            (currentTime - time) > Precision;
    }

    public void DoOnNextBar(UnityAction callback)
    {
        oneTimeBarListeners.Add(callback);
    }

    void BarActions()
    {
        oneTimeBarListeners.ForEach(callback => callback());
        oneTimeBarListeners.Clear();
    }
}
