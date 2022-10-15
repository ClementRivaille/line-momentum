using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fullscreen"))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
    }
}
