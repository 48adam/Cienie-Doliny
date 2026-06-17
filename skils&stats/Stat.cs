using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;
    [SerializeField] private List<int> modifiers = new List<int>();

    public int BaseValue => baseValue;

    public Stat(int baseValue = 0)
    {
        this.baseValue = baseValue;
    }

    public int GetValue()
    {
        int finalValue = baseValue;

        for (int i = 0; i < modifiers.Count; i++)
        {
            finalValue += modifiers[i];
        }

        return finalValue;
    }

    public void SetBaseValue(int value)
    {
        baseValue = value;
    }

    public void AddModifier(int modifier)
    {
        if (modifier != 0)
        {
            modifiers.Add(modifier);
        }
    }

    public void RemoveModifier(int modifier)
    {
        if (modifiers.Contains(modifier))
        {
            modifiers.Remove(modifier);
        }
    }

    public void ClearModifiers()
    {
        modifiers.Clear();
    }
}
