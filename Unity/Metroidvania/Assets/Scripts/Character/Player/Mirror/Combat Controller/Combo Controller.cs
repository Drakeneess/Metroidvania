using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboController
{
    public int maxComboCount=0;
    private int currentComboCount=0;

    public bool canContinueCombo => currentComboCount < maxComboCount;
    public void SetNewMaxCombo(int newMaxCombo)
    {
        if (!canContinueCombo)
        {
            ResetCombo();
        }
        maxComboCount = newMaxCombo;
    }
    public int GetCombo() => currentComboCount;
    public void ResetCombo() => currentComboCount = 0;
    public void ComboFlow(int comboIndex = 1)
    {
        if (currentComboCount >= maxComboCount)
        {
            return;
        }
        currentComboCount += comboIndex;
        Debug.Log($"{currentComboCount} - {maxComboCount}");
    }
}
