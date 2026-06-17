using System;
using UnityEngine;

public class PlayerMagicStats : MonoBehaviour
{
    [Header("Fire Magic")]
    [SerializeField] private int fireLevel = 0;
    [SerializeField] private int fireDamagePerTick = 1;
    [SerializeField] private float fireDuration = 4f;
    [SerializeField] private float fireTickInterval = 1f;

    [Header("Frost Magic")]
    [SerializeField] private int frostLevel = 0;
    [SerializeField] private float frostDuration = 2f;

    public event Action OnMagicStatsChanged;

    public int FireLevel => fireLevel;
    public int FrostLevel => frostLevel;
    public bool HasFire => fireLevel > 0;
    public bool HasFrost => frostLevel > 0;

    public int FireDamagePerTick => Mathf.Max(1, fireDamagePerTick + fireLevel - 1);
    public float FireDuration => Mathf.Max(0.1f, fireDuration);
    public float FireTickInterval => Mathf.Max(0.1f, fireTickInterval);
    public float FrostDuration => Mathf.Max(0.1f, frostDuration);

    public void AddFireLevel(int amount)
    {
        if (amount <= 0)
            return;

        fireLevel += amount;
        OnMagicStatsChanged?.Invoke();
        Debug.Log("Fire level: " + fireLevel);
    }

    public void AddFrostLevel(int amount)
    {
        if (amount <= 0)
            return;

        frostLevel += amount;
        OnMagicStatsChanged?.Invoke();
        Debug.Log("Frost level: " + frostLevel);
    }
}