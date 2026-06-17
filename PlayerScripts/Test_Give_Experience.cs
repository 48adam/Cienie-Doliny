using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Give_Experience : MonoBehaviour
{
    [SerializeField] private Player_Experience playerExperience;
    [SerializeField] private int experienceAmount = 25;
    [SerializeField] private Key testKey = Key.X;

    private void Awake()
    {
        if (playerExperience == null)
            playerExperience = GetComponent<Player_Experience>();
    }

    private void Update()
    {
        if (Game_Pause_Manager.IsPaused)
            return;

        if (Keyboard.current != null && Keyboard.current[testKey].wasPressedThisFrame)
        {
            if (playerExperience != null)
                playerExperience.AddExperience(experienceAmount);
        }
    }
}
