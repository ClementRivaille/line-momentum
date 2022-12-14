using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MusicManager : MonoBehaviour
{
    public bool resetDurations = false;

    public AK.Wwise.Event StartEvent;
    public MusicInfo MusicInfoState;
    public AK.Wwise.RTPC Progress;

    public AK.Wwise.Event SuccessEvent;
    public AK.Wwise.Event FailEvent;

    public AK.Wwise.State EndSong;

    public AK.Wwise.Event LevelComplete;

    public AK.Wwise.RTPC Speed;

    public UnityEvent<float> OnSpeedChange = new UnityEvent<float> ();

    private uint PlayerID;

    public void PlayMusic()
    {
        MusicInfoState.Init();
        PlayerID = StartEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncAll | (uint)AkCallbackType.AK_EnableGetMusicPlayPosition, OnMusicEvent);
    }

    private void OnMusicEvent(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info) {
        if (in_info is AkMusicSyncCallbackInfo)
        {
            AkMusicSyncCallbackInfo musicInfo = (AkMusicSyncCallbackInfo )in_info;
            if (in_type is AkCallbackType.AK_MusicSyncBar)
            {
                MusicInfoState.updatePosition(musicInfo.segmentInfo_iCurrentPosition / 1000f);
                MusicInfoState.onBar.Invoke();

                if (resetDurations)
                {
                    MusicInfoState.barDuration = musicInfo.segmentInfo_fBarDuration;
                    MusicInfoState.beatDuration = musicInfo.segmentInfo_fGridDuration;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        AkSegmentInfo segmentInfo = new AkSegmentInfo();
        AkSoundEngine.GetPlayingSegmentInfo(PlayerID, segmentInfo, true);
        MusicInfoState.updatePosition((float)segmentInfo.iCurrentPosition/1000f);
    }

    public void OnLevelStarted(LevelParams level)
    {
        level.State.SetValue();
        Progress.SetGlobalValue(0);
    }

    public void UpdateProgress(int value)
    {
        Progress.SetGlobalValue(value);
    }

    public void PlaySuccess()
    {
        SuccessEvent.Post(gameObject);
    }

    public void PlayLevelComplete()
    {
        LevelComplete.Post(gameObject);
    }

    public void PlayFail()
    {
        FailEvent.Post(gameObject);
    }

    public void OnGameEnd()
    {
        EndSong.SetValue();
    }

    public void Stop()
    {
        StartEvent.Stop(gameObject);
        SuccessEvent.Post(gameObject);
    }

    public void ChangeMusicSpeed(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        var value = Speed.GetValue(gameObject);
        value = value >= 2f ? 0f : value + 1f;
        Speed.SetGlobalValue(value);

        OnSpeedChange.Invoke(value);
    }
}
