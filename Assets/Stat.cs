using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Stat 
{
    [SerializeField]private int baseValue;

    // 增加伤害效果 BUFF
    public List<int> modifiers;
    public int GetValue()
    {
        int finalValue = baseValue;

        foreach(int modifier in modifiers)
        {
            finalValue -= modifier;
        }
        return finalValue;
    }

    public void SetDefaultVal(int _value)
    {
        baseValue = _value;
    }
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.RemoveAt(_modifier);
    }

    public void MinusValue(int _value)
    {
        baseValue -= _value;
    }
}
