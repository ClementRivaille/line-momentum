using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BeatIndicator))]
public class TransitionButton : MonoBehaviour
{
    private float zPosition;
    private BeatIndicator beat;

    public MusicInfo musicInfoState;
    public AK.Wwise.Event clickSound;

    void Start()
    {
        beat = GetComponent<BeatIndicator>();
        zPosition = transform.localPosition.z;
        transform.localPosition= new Vector3(transform.localPosition.x, transform.localPosition.y, 20f);
    }

    public void Show()
    {
        beat.Reset();
        StartCoroutine(WaitAndShow());
    }

    public void UpdateLevel(LevelParams level)
    {
        Click();
        musicInfoState.DoOnNextBar(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 20f);
            beat.SetColors(level.color, level.barColor);
        });
    }

    public void ValidateEnd()
    {
        Click();
        musicInfoState.DoOnNextBar(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 20f);
        });
    }

    void Click()
    {
        beat.Validate();
        clickSound.Post(gameObject);
    }

    IEnumerator WaitAndShow()
    {
        yield return new WaitForEndOfFrame();
        musicInfoState.DoOnNextBar(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zPosition);
        });
    }
}
