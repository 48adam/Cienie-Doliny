using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Experience_UI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player_Experience playerExperience;

    [Header("UI")]
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text experienceText;

    [Header("Display")]
    [SerializeField] private bool showExperienceNumbers = true;
    [SerializeField] private string levelPrefix = "Level: ";

    private void Awake()
    {
        if (playerExperience == null)
            playerExperience = FindFirstObjectByType<Player_Experience>();
    }

    private void OnEnable()
    {
        if (playerExperience != null)
        {
            playerExperience.OnExperienceChanged += UpdateExperienceUI;
            playerExperience.OnLevelChanged += UpdateLevelUI;
        }
    }

    private void OnDisable()
    {
        if (playerExperience != null)
        {
            playerExperience.OnExperienceChanged -= UpdateExperienceUI;
            playerExperience.OnLevelChanged -= UpdateLevelUI;
        }
    }

    private void Start()
    {
        if (playerExperience != null)
        {
            UpdateExperienceUI(
                playerExperience.CurrentExperience,
                playerExperience.ExperienceToNextLevel,
                playerExperience.CurrentLevel
            );

            UpdateLevelUI(playerExperience.CurrentLevel);
        }
    }

    private void UpdateExperienceUI(int currentExperience, int experienceToNextLevel, int currentLevel)
    {
        if (experienceSlider != null)
        {
            experienceSlider.minValue = 0;
            experienceSlider.maxValue = experienceToNextLevel;
            experienceSlider.value = currentExperience;
        }

        if (experienceText != null)
        {
            experienceText.text = showExperienceNumbers
                ? currentExperience + " / " + experienceToNextLevel + " XP"
                : "";
        }

        UpdateLevelUI(currentLevel);
    }

    private void UpdateLevelUI(int currentLevel)
    {
        if (levelText != null)
            levelText.text = levelPrefix + currentLevel;
    }
}
