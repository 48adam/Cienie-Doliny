using UnityEngine;

public static class GameRunStats
{
    public static int EnemiesKilled { get; private set; }
    public static int WavesSurvived { get; private set; }

    public static void ResetStats()
    {
        EnemiesKilled = 0;
        WavesSurvived = 0;
    }

    public static void RegisterEnemyKilled()
    {
        EnemiesKilled++;
        Debug.Log("Pokonani przeciwnicy: " + EnemiesKilled);
    }

    public static void RegisterWaveSurvived()
    {
        WavesSurvived++;
        Debug.Log("Przetrwane fale: " + WavesSurvived);
    }
}