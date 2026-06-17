using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum SkillEffectType
{
    None,
    MaxHealth,
    Damage,
    MoveSpeed,
    Armor,
    Fire,
    Frost
}
public class Skill_Node_UI : MonoBehaviour
{
    [Header("Skill Info")]
    [SerializeField] private string skillName = "New Skill";
    [SerializeField] private string description = "Skill description";
    [SerializeField] private int cost = 1;
    [SerializeField] private int maxRank = 1;
    [SerializeField] private Sprite icon;

    [Header("Skill Effect")]
    [SerializeField] private SkillEffectType effectType = SkillEffectType.None;
    [SerializeField] private int effectAmount = 1;

    [Header("Requirements")]
    [SerializeField] private Skill_Node_UI[] prerequisiteSkills;

    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject unlockedOverlay;

    [Header("Runtime State")]
    [SerializeField] private int currentRank = 0;

    private Skill_Tree_Manager manager;

    public string SkillName => skillName;
    public int CurrentCost => Mathf.Max(1, cost);
    public int MaxRank => Mathf.Max(1, maxRank);
    public int CurrentRank => currentRank;
    public int EffectAmount => effectAmount;
    public SkillEffectType EffectType => effectType;
    public bool IsUnlocked => currentRank > 0;
    public bool IsMaxed => currentRank >= MaxRank;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void SetManager(Skill_Tree_Manager newManager)
    {
        manager = newManager;
        RefreshUI();
    }

    public void OnSkillClicked()
    {
        Debug.Log("Klikniêto przycisk skilla: " + skillName);

        if (manager == null)
        {
            manager = GetComponentInParent<Skill_Tree_Manager>();

            if (manager == null)
                manager = FindFirstObjectByType<Skill_Tree_Manager>();
        }

        if (manager == null)
        {
            Debug.LogWarning("Brakuje Skill_Tree_Manager dla skilla: " + skillName);
            return;
        }

        manager.TryBuySkill(this);
    }

    public void LevelUpSkill()
    {
        if (IsMaxed)
            return;

        currentRank++;
        RefreshUI();
    }

    public bool ArePrerequisitesUnlocked()
    {
        if (prerequisiteSkills == null || prerequisiteSkills.Length == 0)
            return true;

        for (int i = 0; i < prerequisiteSkills.Length; i++)
        {
            if (prerequisiteSkills[i] != null && !prerequisiteSkills[i].IsUnlocked)
                return false;
        }

        return true;
    }

    public void RefreshUI()
    {
        bool requirementsMet = ArePrerequisitesUnlocked();
        bool canAfford = manager == null || manager.CurrentSkillPoints >= CurrentCost;
        bool canClick = !IsMaxed && requirementsMet && canAfford;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        if (nameText != null)
            nameText.text = skillName;

        if (descriptionText != null)
            descriptionText.text = description;

        if (costText != null)
            costText.text = IsMaxed ? "MAX" : "Cost: " + CurrentCost;

        if (levelText != null)
            levelText.text = currentRank + "/" + MaxRank;

        if (lockedOverlay != null)
            lockedOverlay.SetActive(!requirementsMet || !canAfford);

        if (unlockedOverlay != null)
            unlockedOverlay.SetActive(IsUnlocked);

        if (button != null)
            button.interactable = canClick;
    }
}
