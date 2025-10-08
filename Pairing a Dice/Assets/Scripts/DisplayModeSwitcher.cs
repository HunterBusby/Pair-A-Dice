using UnityEngine;

public class DisplayModeSwitcher : MonoBehaviour
{
    public enum Mode { Windowed, BorderlessFullscreen, ExclusiveFullscreen }

    public void SetMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Windowed:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            case Mode.BorderlessFullscreen:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // borderless
                break;

        }
    }

    // If you prefer simple buttons:
    public void SetWindowed()             => SetMode(Mode.Windowed);
    public void SetBorderlessFullscreen() => SetMode(Mode.BorderlessFullscreen);
}
