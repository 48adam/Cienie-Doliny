using UnityEngine;

public class Stat_Upgrade_Button : MonoBehaviour
{
    [SerializeField] private Stats targetStats;
    [SerializeField] private StatType statToUpgrade = StatType.Damage;
    [SerializeField] private int upgradeAmount = 1;

    private void Awake()
    {
        if (targetStats == null)
            targetStats = FindFirstObjectByType<Stats>();
    }

    // Podepnij tę metodę pod Button -> OnClick().
    public void UpgradeStat()
    {
        if (targetStats == null)
        {
            Debug.LogWarning("Brakuje referencji do Stats w Stat_Upgrade_Button.");
            return;
        }

        targetStats.AddModifier(statToUpgrade, upgradeAmount);
    }
}
