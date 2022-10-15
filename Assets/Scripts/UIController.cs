using DigitalRuby.Tween;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public MusicInfo MusicInfoState;

    public CanvasGroup StartScreen;
    public CanvasGroup Instructions;
    public CanvasGroup Credits;

    private LevelParams incomingLevel;

    public UnityEvent OnCreditsFinished = new UnityEvent();

    private int creditsBar = 0;
    private bool creditsActive = false;
    private int[] creditsThreshold = { 0, 2, 5, 8 };

    [Serializable]
    public class InstructionText
    {
        public LevelId level;
        public Text text;

        [NonSerialized]
        public bool displayed = false;
    }
    public List<InstructionText> Tutorials;

    void Start()
    {
        MusicInfoState.onBar.AddListener(BarUpdate);
    }

    void BarUpdate() {
        if (incomingLevel != null) {
            var instruction = Tutorials.Find(t => t.level == incomingLevel.id);
            if (instruction != null && !instruction.displayed)
            {
                StartCoroutine(DisplayInstruction(instruction));
            }
            incomingLevel = null;
        } if (creditsActive)
        {
            creditsBar += 1;
            var nextCreditSlide = Array.IndexOf(creditsThreshold, creditsBar);
            if (nextCreditSlide > 0)
            {
                StartCoroutine(DisplayCreditPage(nextCreditSlide));
                if (nextCreditSlide == creditsThreshold.Length - 1)
                {
                    creditsActive = false;
                    OnCreditsFinished.Invoke();
                }
            }
        }
    }

    public void StartGame() {
        if (StartScreen.alpha > 0)
        {
            FadeScreen(StartScreen, 0.4f, false);
        } else if (Credits.alpha > 0)
        {
            TweenFactory.Clear();
            Credits.alpha = 0f;
            foreach (Transform creditSlide in Credits.transform)
            {
                creditSlide.gameObject.SetActive(false);
            }
        }
    }

    private WaitForSeconds FadeScreen(CanvasGroup screen, float duration, bool fadeIn)
    {
        var tween = screen.gameObject.Tween("Fade screen", screen.alpha,
            fadeIn ? 1f : 0f,
            duration,
            fadeIn ? TweenScaleFunctions.SineEaseOut : TweenScaleFunctions.SineEaseIn,
            (t) => {
               screen.alpha = t.CurrentValue;
             }
        );

        return new WaitForSeconds(duration);
    }

    public void StartLevel(LevelParams level)
    {
        incomingLevel = level;
    }

    private IEnumerator DisplayInstruction(InstructionText instruction)
    {
        instruction.displayed = true;
        Instructions.alpha = 0f;
        instruction.text.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.6f);
        yield return FadeScreen(Instructions, 0.6f, true);
        yield return new WaitForSeconds(5f);
        yield return FadeScreen(Instructions, 0.5f, false);

        instruction.text.gameObject.SetActive(false);
    }

    public void StartCredits()
    {
        creditsActive = true;
        creditsBar = 0;
        Credits.alpha = 1f;
        Credits.transform.GetChild(0).gameObject.SetActive(true);
    }

    private IEnumerator DisplayCreditPage(int idx)
    {
        yield return FadeScreen(Credits, 0.6f, false);
        Credits.transform.GetChild(Math.Max(0, idx - 1)).gameObject.SetActive(false);
        Credits.transform.GetChild(idx).gameObject.SetActive(true);
        yield return FadeScreen(Credits, 0.6f, true);
    }
}
