using System;
using UnityEngine;

public class Player_Experience : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExperience = 0;
    [SerializeField] private int experienceToNextLevel = 100;

    [Header("Skill Points")]
    [SerializeField] private int currentSkillPoints = 0;
    [SerializeField] private int skillPointsPerLevel = 1;

    [Header("Scaling")]
    [SerializeField] private float levelExperienceMultiplier = 1.5f;
    [SerializeField] private int minimumExperienceToNextLevel = 10;

    [Header("Optional Rewards")]
    [SerializeField] private Stats stats;
    [SerializeField] private bool increaseStatsOnLevelUp = false;
    [SerializeField] private int damagePerLevel = 1;
    [SerializeField] private int maxHealthPerLevel = 5;

    public event Action<int, int, int> OnExperienceChanged;
    public event Action<int> OnLevelChanged;
    public event Action<int> OnSkillPointsChanged;

    public int CurrentLevel => currentLevel;
    public int CurrentExperience => currentExperience;
    public int ExperienceToNextLevel => experienceToNextLevel;
    public int CurrentSkillPoints => currentSkillPoints;
    public float ExperienceProgress => experienceToNextLevel <= 0 ? 0f : (float)currentExperience / experienceToNextLevel;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<Stats>();

        experienceToNextLevel = Mathf.Max(minimumExperienceToNextLevel, experienceToNextLevel);
        currentLevel = Mathf.Max(1, currentLevel);
        currentExperience = Mathf.Max(0, currentExperience);
        currentSkillPoints = Mathf.Max(0, currentSkillPoints);
        skillPointsPerLevel = Mathf.Max(0, skillPointsPerLevel);
    }

    private void Start()
    {
        NotifyExperienceChanged();
        OnLevelChanged?.Invoke(currentLevel);
        NotifySkillPointsChanged();
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0)
            return;

        currentExperience += amount;

        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            LevelUp();
        }

        NotifyExperienceChanged();
    }

    private void LevelUp()
    {
        currentLevel++;
        experienceToNextLevel = Mathf.Max(
            minimumExperienceToNextLevel,
            Mathf.RoundToInt(experienceToNextLevel * levelExperienceMultiplier)
        );

        AddSkillPoints(skillPointsPerLevel);

        if (increaseStatsOnLevelUp && stats != null)
        {
            stats.AddModifier(StatType.Damage, damagePerLevel);
            stats.AddModifier(StatType.MaxHealth, maxHealthPerLevel);
        }

        OnLevelChanged?.Invoke(currentLevel);
        Debug.Log("Level up! Aktualny level: " + currentLevel + ". Skill points: " + currentSkillPoints);
    }

    public void AddSkillPoints(int amount)
    {
        if (amount <= 0)
            return;

        currentSkillPoints += amount;
        NotifySkillPointsChanged();
    }

    public bool CanSpendSkillPoints(int amount)
    {
        return amount > 0 && currentSkillPoints >= amount;
    }

    public bool SpendSkillPoints(int amount)
    {
        if (!CanSpendSkillPoints(amount))
            return false;

        currentSkillPoints -= amount;
        NotifySkillPointsChanged();
        return true;
    }

    private void NotifyExperienceChanged()
    {
        OnExperienceChanged?.Invoke(currentExperience, experienceToNextLevel, currentLevel);
    }

    private void NotifySkillPointsChanged()
    {
        OnSkillPointsChanged?.Invoke(currentSkillPoints);
    }
}
