using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WindowManager : MonoBehaviour
{

    public bool CursorVisible = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = CursorVisible;
    }

    public void ToggleFullScreen(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        bool fullScreen = !Screen.fullScreen;
        if (fullScreen)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        } else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1280, 720, FullScreenMode.FullScreenWindow);
        }
        Screen.fullScreen = fullScreen;
    }

    public void Quit(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        Application.Quit();
    }
}
