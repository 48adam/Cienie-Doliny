using UnityEngine;

public static class Game_Pause_Manager
{
    public static bool IsPaused { get; private set; }

    public static void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }

    public static void TogglePause()
    {
        SetPaused(!IsPaused);
    }

    public static void ForceUnpause()
    {
        SetPaused(false);
    }
}
