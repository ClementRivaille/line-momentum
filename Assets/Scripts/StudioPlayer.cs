using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StudioPlayer : MonoBehaviour
{
    public Color backgroundColor;
    public Color lineColor;

    public AK.Wwise.Event toggleSound;
    public AK.Wwise.Event untoggleSound;

    void Start()
    {
        Cursor.visible = true;
        var buttons = GameObject.FindGameObjectsWithTag("button").ToArray().Concat(GameObject.FindGameObjectsWithTag("radio").ToArray());
        foreach (var button in buttons)
        {
            var sprite = button.GetComponent<BeatIndicator>();
            sprite.SetColors(backgroundColor, lineColor);
            var toggle = button.GetComponent<ToggleButton>();
            toggle.OnToggle += OnToggle;
        }
    }

    public void UpdateSwitchValue(AK.Wwise.Switch param)
    {
        param.SetValue(gameObject);
    }

    void OnToggle(bool value)
    {
        if (value)
        {
            toggleSound.Post(gameObject);
        } else
        {
            untoggleSound.Post(gameObject);
        }
    }
}
