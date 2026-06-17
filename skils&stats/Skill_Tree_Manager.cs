using TMPro;
using UnityEngine;

public class Skill_Tree_Manager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player_Experience playerExperience;
    [SerializeField] private Stats playerStats;
    [SerializeField] private TMP_Text skillPointsText;

    [Header("Nodes")]
    [SerializeField] private Skill_Node_UI[] skillNodes;

    [SerializeField] private PlayerMagicStats playerMagicStats;

    public int CurrentSkillPoints => playerExperience != null ? playerExperience.CurrentSkillPoints : 0;

    private void Awake()
    {
        if (playerExperience == null)
            playerExperience = FindFirstObjectByType<Player_Experience>();

        if (playerStats == null)
            playerStats = FindFirstObjectByType<Stats>();

        if (playerMagicStats == null)
            playerMagicStats = FindFirstObjectByType<PlayerMagicStats>();

        if (skillNodes == null || skillNodes.Length == 0)
            skillNodes = GetComponentsInChildren<Skill_Node_UI>(true);

        for (int i = 0; i < skillNodes.Length; i++)
        {
            if (skillNodes[i] != null)
                skillNodes[i].SetManager(this);
        }
    }

    private void OnEnable()
    {
        if (playerExperience != null)
            playerExperience.OnSkillPointsChanged += OnSkillPointsChanged;
    }

    private void OnDisable()
    {
        if (playerExperience != null)
            playerExperience.OnSkillPointsChanged -= OnSkillPointsChanged;
    }

    private void Start()
    {
        RefreshAllNodes();
        UpdateSkillPointsText();
    }

    public bool TryBuySkill(Skill_Node_UI node)
    {
        if (node == null)
            return false;

        if (node.IsMaxed)
        {
            Debug.Log("Skill jest już maksymalnie ulepszony: " + node.SkillName);
            return false;
        }

        if (!node.ArePrerequisitesUnlocked())
        {
            Debug.Log("Najpierw odblokuj poprzednie skille dla: " + node.SkillName);
            return false;
        }

        int cost = node.CurrentCost;

        if (playerExperience == null || !playerExperience.SpendSkillPoints(cost))
        {
            Debug.Log("Brakuje punktów skilla. Potrzebujesz: " + cost);
            return false;
        }

        node.LevelUpSkill();
        ApplySkillEffect(node);
        RefreshAllNodes();
        UpdateSkillPointsText();

        Debug.Log("Kupiono skill: " + node.SkillName + " | Rank: " + node.CurrentRank + "/" + node.MaxRank);
        return true;
    }

    private void ApplySkillEffect(Skill_Node_UI node)
    {
        if (playerStats == null || node == null)
            return;

        int amount = node.EffectAmount;

        switch (node.EffectType)
        {
            case SkillEffectType.MaxHealth:
                playerStats.AddModifier(StatType.MaxHealth, amount);
                break;

            case SkillEffectType.Damage:
                playerStats.AddModifier(StatType.Damage, amount);
                break;

            case SkillEffectType.MoveSpeed:
                playerStats.AddModifier(StatType.MoveSpeed, amount);
                break;

            case SkillEffectType.Armor:
                playerStats.AddModifier(StatType.Armor, amount);
                break;

            case SkillEffectType.Fire:
                if (playerMagicStats != null)
                    playerMagicStats.AddFireLevel(amount);
                break;

            case SkillEffectType.Frost:
                if (playerMagicStats != null)
                    playerMagicStats.AddFrostLevel(amount);
                break;

        }
    }

    private void OnSkillPointsChanged(int points)
    {
        RefreshAllNodes();
        UpdateSkillPointsText();
    }

    public void RefreshAllNodes()
    {
        if (skillNodes == null)
            return;

        for (int i = 0; i < skillNodes.Length; i++)
        {
            if (skillNodes[i] != null)
                skillNodes[i].RefreshUI();
        }
    }

    private void UpdateSkillPointsText()
    {
        if (skillPointsText != null)
            skillPointsText.text = "skill points: " + CurrentSkillPoints;
    }

    // Testowe dodawanie punktów z przycisku UI, jeśli chcesz szybko sprawdzać drzewko.
    public void AddOneSkillPointForTesting()
    {
        if (playerExperience != null)
            playerExperience.AddSkillPoints(1);
    }
}
