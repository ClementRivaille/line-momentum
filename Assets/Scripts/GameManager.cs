using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public UnityEvent OnLevelEnd = new UnityEvent();
    public UnityEvent OnGameEnd = new UnityEvent();
    public UnityEvent OnStartCredit = new UnityEvent();
    public UnityEvent<bool> OnUpdatePerfect = new UnityEvent<bool>();
    public UnityEvent<bool> OnSetPerfectionistMode = new UnityEvent<bool>();

    private int currentLevel = -1;

    [HideInInspector]
    public int progress = 0;
    private int nextBeatIdx = -1;
    private float nextBeatPosition = 0.0f;
    private bool mistakes = false;
    private bool barEnded = false;
    private bool levelSuccessFul = false;
    private bool gameFinished = false;
    private int transitionBar = 0;

    public UnityEvent<bool> OnAccessibilityUpdate = new UnityEvent<bool>();
    private bool accessibilytyMode = false;

    private bool perfectionistMode = false;
    [HideInInspector]
    public List<bool> levelsPerfect = new List<bool>();
    private bool isPerfect = true;

    enum GameState
    {
        StartScreen,
        Playing,
        EndLevel,
        Credits,
        End
    }
    private GameState state = GameState.StartScreen;

     // Start is called before the first frame update
    void Start()
    {
        MusicInfoState.onBar.AddListener(StartBar);
        levelsPerfect = levels.Select(x => false).ToList();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if (state == GameState.StartScreen)
        {
            InitLevel(0);
            transitionBar = 2;
            state = GameState.Playing;
            OnStartGame.Invoke();
        }
        else if (state == GameState.Playing && transitionBar <= 0)
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
                    UpdateProgress((int)ProgressNeeded - 2);
                    levelSuccessFul = false;
                }
            }
        }
        else if (state == GameState.EndLevel && transitionBar <= 0)
        {
            GoToNextLevel();
        }
        else if (state == GameState.End)
        {
            perfectionistMode = true;
            OnSetPerfectionistMode.Invoke(true);
            InitLevel(0);
            transitionBar = 2;
            state = GameState.Playing;
            OnStartGame.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (state == GameState.Playing && transitionBar <= 0) {
            if (!barEnded && MusicInfoState.IsPassed(nextBeatPosition)) {
                MissBeat();
            }
        }
    }

    void InitLevel(int idx, bool keepProgress = false)
    {
        currentLevel = idx;
        UpdateProgress(keepProgress ? progress : 0);
        levelSuccessFul = false;
        UpdatePerfect(true);
        levelsPerfect[currentLevel] = false;
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
        
        if (transitionBar > 0)
        {
            transitionBar -= 1;
        }

        if (state == GameState.Playing && progress == 0)
        {
            UpdatePerfect(true);
        }
        if (state == GameState.Playing && levelSuccessFul)
        {
            state = GameState.EndLevel;
            levelsPerfect[currentLevel] = isPerfect;
            transitionBar = 1;
            OnLevelEnd.Invoke();
        }
        else if (gameFinished)
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
            if (!perfectionistMode)
            {
                UpdatePerfect(progress > 0 ? false : true);
                UpdateProgress(Math.Max(0, progress - (barEnded ? 2 : 1)));
            } else
            {
                UpdateProgress(0);
            }
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
            levelSuccessFul = progress == ProgressNeeded;
        }
    }

    void GoToNextLevel()
    {
        var nextLevelIdx = (currentLevel + 1) % levels.Count;
        if (nextLevelIdx > 0)
        {
            InitLevel(nextLevelIdx);
            transitionBar = 2;
            state = GameState.Playing;
        }
        else
        {
            gameFinished = true;
            OnGameEnd.Invoke();
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

    void UpdatePerfect(bool value)
    {
        isPerfect = value;
        OnUpdatePerfect.Invoke(value);
    }

    public void CreditsEnded()
    {
        state = GameState.End;
    }

    public void OnToggleAccessibility(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        accessibilytyMode = !accessibilytyMode;
        OnAccessibilityUpdate.Invoke(accessibilytyMode);
    }

    public void OnTogglePerfectionnist(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        perfectionistMode = !perfectionistMode;
        OnSetPerfectionistMode.Invoke(perfectionistMode);
    }
}
