using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StudioPlayer : MonoBehaviour
{
    public Color backgroundColor;
    public Color lineColor;

    void Start()
    {
        Cursor.visible = true;
        var buttons = GameObject.FindGameObjectsWithTag("button").ToArray().Concat(GameObject.FindGameObjectsWithTag("radio").ToArray());
        foreach (var button in buttons)
        {
            var sprite = button.GetComponent<BeatIndicator>();
            sprite.SetColors(backgroundColor, lineColor);
        }
    }

    public void UpdateSwitchValue(AK.Wwise.Switch param)
    {
        param.SetValue(gameObject);
    }
}
