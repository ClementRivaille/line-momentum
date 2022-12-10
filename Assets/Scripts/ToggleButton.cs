using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum ToggleMode
{
    CheckBox,
    Radio
}

[RequireComponent(typeof(BeatIndicator), typeof(Collider2D))]
public class ToggleButton : MonoBehaviour, IPointerDownHandler
{

    private BeatIndicator sprite;

    public bool toggled = false;
    public bool disabled = false;
    public ToggleMode mode = ToggleMode.CheckBox;
    public List<AK.Wwise.State> disableOn;

    public AK.Wwise.Switch checkedSwitch = null;
    public AK.Wwise.Switch unheckedSwitch = null;
    public AK.Wwise.State selectState = null;

    public UnityEvent<AK.Wwise.Switch> SetSwitchValue = new UnityEvent<AK.Wwise.Switch>();

    public UnityAction<bool> OnToggle;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<BeatIndicator>();
        if (toggled)
        {
            sprite.Validate();
            if (mode == ToggleMode.CheckBox)
            {
                SetSwitchValue.Invoke(checkedSwitch);
            }
        }
        if (disabled)
        {
            sprite.Fail();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mode == ToggleMode.CheckBox && !disabled)
        {
            toggled = !toggled;
            OnToggleValueChanged();
            OnToggle.Invoke(toggled);
        } else if (mode == ToggleMode.Radio && !toggled)
        {
            toggled = true;
            OnToggleValueChanged();
            OnToggle.Invoke(true);

            GameObject[] radios = GameObject.FindGameObjectsWithTag("radio");
            foreach (GameObject radio in radios)
            {
                ToggleButton toggle = radio.GetComponent<ToggleButton>();
                if (toggle.selectState != selectState)
                {
                    toggle.toggled = false;
                    toggle.OnToggleValueChanged();
                }
            }
        }
    }

    public void OnToggleValueChanged()
    {
        if (toggled)
        {
            sprite.Validate();
        } else
        {
            sprite.Reset();
        }

        if (toggled && mode == ToggleMode.Radio)
        {
            selectState.SetValue();

            GameObject[] checkboxes = GameObject.FindGameObjectsWithTag("button");
            foreach (var obj in checkboxes)
            {
                ToggleButton checkbox = obj.GetComponent<ToggleButton>();
                checkbox.onChangeLevel(selectState);
            }
        }
        else
        {
            SetSwitchValue.Invoke(toggled ? checkedSwitch : unheckedSwitch);
        }
    }

    public void onChangeLevel(AK.Wwise.State level)
    {
        if (disableOn.Exists(l => level.Name == l.Name))
        {
            if (!disabled)
            {
                disabled = true;
                sprite.Fail();
            }
        } else if (disabled)
        {
            disabled = false;
            if (toggled)
            {
                sprite.Validate();
            } else
            {
                sprite.Reset();
            }
        }
    }
}
