using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum LevelId
{
    Level1,
    Level2,
    Level3,
    Level4,
}

[Serializable]
public class LevelParams
{
    public LevelId id;
    public Color color;
    public Color barColor;
    public AK.Wwise.State State;
    public List<uint> beats;
}

public class GameManager : MonoBehaviour
{
    public List<LevelParams> levels;
    public uint ProgressNeeded = 10;

    public MusicInfo MusicInfoState;

    public UnityEvent<LevelParams> OnStartLevel = new UnityEvent<LevelParams>();
    public UnityEvent OnValidateBeat = new UnityEvent();
    public UnityEvent OnFail = new UnityEvent();
    public UnityEvent OnMissBeat = new UnityEvent();
    public UnityEvent OnStartGame = new UnityEvent();
    public UnityEvent<int> OnUpdateProgress = new UnityEvent<int>();
    public UnityEvent OnGameEnd = new UnityEvent();
    public UnityEvent OnStartCredit = new UnityEvent();

    private int currentLevel = -1;

    public int progress = 0;
    private int nextBeatIdx = -1;
    private float nextBeatPosition = 0.0f;
    private bool mistakes = false;
    private bool barEnded = false;
    private bool levelSuccessFul = false;
    private bool gameFinished = false;

    enum GameState
    {
        StartScreen,
        Playing,
        Credits,
        End
    }
    private GameState state = GameState.StartScreen;

     // Start is called before the first frame update
    void Start()
    {
        MusicInfoState.onBar.AddListener(StartBar);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if (state == GameState.StartScreen)
        {
            InitLevel(0);
            state = GameState.Playing;
            OnStartGame.Invoke();
        }
        else if (state == GameState.Playing)
        {
            if (MusicInfoState.IsCloseTo(nextBeatPosition))
            {
                ValidateBeat();
            }
            else if (!MusicInfoState.IsCloseTo(0f))
            {
                FailBar();
                OnFail.Invoke();

                if (levelSuccessFul)
                {
                    InitLevel(Math.Max(0, gameFinished ? currentLevel : currentLevel - 1), true);
                    UpdateProgress((int)ProgressNeeded - 2);
                    levelSuccessFul = false;
                    gameFinished = false;
                }
            }
        }
        else if (state == GameState.End)
        {
            InitLevel(0);
            state = GameState.Playing;
            OnStartGame.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (state == GameState.Playing) {
            if (!barEnded && MusicInfoState.IsPassed(nextBeatPosition)) {
                MissBeat();
            }
        }
    }

    void InitLevel(int idx, bool keepProgress = false)
    {
        currentLevel = idx;
        UpdateProgress(keepProgress ? progress : 0);
        nextBeatIdx = 0;
        UpdateNextBeatPosition();
        OnStartLevel.Invoke(levels[idx]);
    }

    void UpdateNextBeatPosition()
    {
        var level = levels[currentLevel];
        nextBeatPosition = level.beats[nextBeatIdx] * MusicInfoState.beatDuration;
    }

    public void StartBar()
    {
        barEnded = false;
        mistakes = false;
        levelSuccessFul = false;

        if (gameFinished)
        {
            state = GameState.Credits;
            gameFinished = false;
            OnStartCredit.Invoke();
        }
    }

    void FailBar()
    {
        if (!mistakes)
        {
            mistakes = true;
            UpdateProgress(Math.Max(0, progress - (barEnded ? 2 : 1)));
        }
    }

    void ValidateBeat()
    {
        var level = levels[currentLevel];
        nextBeatIdx = (nextBeatIdx + 1) % level.beats.Count;
        UpdateNextBeatPosition();
        OnValidateBeat.Invoke();

        barEnded = nextBeatIdx == 0;
        if (barEnded && !mistakes)
        {
            UpdateProgress(progress + 1);

            if (progress == ProgressNeeded)
            {
                var nextLevelIdx = (currentLevel + 1) % levels.Count;
                if (nextLevelIdx > 0)
                {
                    InitLevel(nextLevelIdx);
                }
                else
                {
                    gameFinished = true;
                    OnGameEnd.Invoke();
                }
                levelSuccessFul = true;
            }
        }
    }

    void MissBeat()
    {
        FailBar();
        nextBeatIdx = (nextBeatIdx + 1) % levels[currentLevel].beats.Count;
        UpdateNextBeatPosition();
        OnMissBeat.Invoke();

        barEnded = nextBeatIdx == 0;
    }

    void UpdateProgress(int value)
    {
        progress = value;
        OnUpdateProgress.Invoke(value);
    }

    public void CreditsEnded()
    {
        state = GameState.End;
    }
}
