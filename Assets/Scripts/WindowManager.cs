using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WindowManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;   
    }

    public void ToggleFullScreen(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void Quit(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        Application.Quit();
    }
}
