using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Background : MonoBehaviour
{
    public MusicInfo MusicInfoState;

    private SpriteRenderer sprite;
    private Color nextColor;
    private Color startColor;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        startColor = sprite.color;
    }

    public void UpdateColor(LevelParams level)
    {
        nextColor = level.color;
        MusicInfoState.onBar.AddListener(SetColor);
    }

    void SetColor()
    {
        sprite.color = nextColor;
        MusicInfoState.onBar.RemoveListener(SetColor);
    }

    public void ResetColor()
    {
        sprite.color = startColor;
    }
}
