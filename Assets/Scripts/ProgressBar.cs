using DigitalRuby.Tween;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProgressBar : MonoBehaviour
{
    public MusicInfo MusicInfoState;
    public RectTransform CurrentPositionDot;

    public RectTransform BeatPrefab;
    public RectTransform CrossPrefab;

    private bool active = false;
    private bool autoValidateFirstBeat = false;
    private List<BeatIndicator> beats = new List<BeatIndicator>();
    private List<RectTransform> crosses = new List<RectTransform>();
    private LevelParams nextLevel;

    public SpriteRenderer sprite;

    private bool displayPosition = true;
    private float dotLightIntensity;
    public int DotDisappearanceThreshold = 5;
    private Light2D dotLight;

    void Start()
    {
        dotLight = CurrentPositionDot.GetComponent<Light2D>();
        dotLightIntensity = dotLight.intensity;

        MusicInfoState.onMusicStarted.AddListener(OnMusicStart);
        CurrentPositionDot.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (active)
        {
            var DotPosition = MusicInfoState.barPosition;
            CurrentPositionDot.anchorMin = new Vector2(DotPosition, 0.5f);
            CurrentPositionDot.anchorMax = new Vector2(DotPosition, 0.5f);
            CurrentPositionDot.anchoredPosition = Vector2.zero;
        }
    }

    public void OnMusicStart()
    {
        CurrentPositionDot.gameObject.SetActive(true);
        active = true;
        MusicInfoState.onBar.AddListener(UpdateBar);
    }

    public void OnChangeLevel(LevelParams level)
    {
        nextLevel = level;
    }

    void UpdateBar()
    {
        beats.ForEach(beat => beat.Reset());
        crosses.ForEach(cross => Destroy(cross.gameObject));
        crosses.Clear();
        if (nextLevel != null)
        {
            UpdateLevel();
        }

        if (autoValidateFirstBeat)
        {
            beats[0].Validate();
            autoValidateFirstBeat = false;
        }

        if (!displayPosition && dotLight.intensity > 0)
        {
            FadeOutDot();
        } else if (displayPosition && dotLight.intensity == 0)
        {
            dotLight.intensity = dotLightIntensity;
            CurrentPositionDot.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    void UpdateLevel()
    {
        sprite.color = nextLevel.barColor;

        beats.ForEach((beat) => Destroy(beat.gameObject));
        beats.Clear();
        for (var i = 0; i < nextLevel.beats.Count; i++)
        {
            var beat = Instantiate(BeatPrefab, transform);
            float beatPosition = (float)nextLevel.beats[i] / 10f;
            beat.anchorMin = new Vector2(beatPosition, 0.5f);
            beat.anchorMax = new Vector2(beatPosition, 0.5f);
            beat.anchoredPosition = Vector2.zero;

            var beatIndicator = beat.GetComponent<BeatIndicator>();
            beatIndicator.SetColors(nextLevel.color, nextLevel.barColor);
            beats.Add(beatIndicator);
        }

        nextLevel = null;
    }

    public void ValidateNextBeat()
    {
        var nextBeat = beats.Find(beat => beat.status == BeatStatus.Pending);
        if (nextBeat != null)
        {
            nextBeat.Validate();
        }
        else
        {
            autoValidateFirstBeat = true;
        }
    }

    public void MissBeat()
    {
        var nextBeat = beats.Find(beat => beat.status == BeatStatus.Pending);
        if (nextBeat == null) return;
        nextBeat.Fail();
    }

    public void Fail()
    {
        var cross = Instantiate(CrossPrefab, transform);
        float crossPosition = MusicInfoState.barPosition;
        cross.anchorMin = new Vector2(crossPosition, 0.5f);
        cross.anchorMax = new Vector2(crossPosition, 0.5f);
        cross.anchoredPosition = Vector2.zero;
        crosses.Add(cross);
    }

    public void Hide()
    {
        beats.ForEach((beat) => Destroy(beat.gameObject));
        beats.Clear();
        active = false;
        CurrentPositionDot.gameObject.SetActive(false);
        MusicInfoState.onBar.RemoveListener(UpdateBar);
        sprite.color = new Color(0, 0, 0, 0);
    }

    public void ProgressUpdate(int progress)
    {
        displayPosition = progress < DotDisappearanceThreshold;
    }

    private void FadeOutDot()
    {
        var dotSprite = CurrentPositionDot.GetComponent<SpriteRenderer>();
        dotSprite.gameObject.Tween("SpriteFadeOut", Color.white, new Color(1f, 1f, 1f, 0f),
            1f, TweenScaleFunctions.Linear,
            t => dotSprite.color = t.CurrentValue);
        dotLight.gameObject.Tween("SpriteLightFadeOut", dotLightIntensity, 0f,
            1f, TweenScaleFunctions.Linear,
            t => dotLight.intensity = t.CurrentValue);
    }
}
