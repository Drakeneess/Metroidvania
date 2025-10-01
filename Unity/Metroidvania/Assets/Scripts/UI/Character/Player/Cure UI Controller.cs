using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureUIController : MonoBehaviour
{
    [Header("Prefab/Container")]
    [SerializeField] private CureUI cureUIBase;
    [SerializeField] private Transform container;

    private readonly List<CureUI> slots = new();
    private CureController cures;

    private int current;
    private int max;

    private int healingIndex = -1;
    private bool isHealing = false;

    void OnEnable()
    {
        StartCoroutine(WaitForCureController());
    }

    void OnDisable()
    {
        if (cures == null) return;
        cures.OnCureUsed -= OnEventCureUsed;
        cures.OnCureUpgraded -= OnEventCureUpgraded;
        cures.OnCureRestored -= OnEventCureRestored;
        cures.OnHealingStarted -= OnHealingStarted;
        cures.OnHealingFinished -= OnHealingFinished;
    }

    private IEnumerator WaitForCureController()
    {
        while (CureController.Instance == null)
            yield return null;

        cures = CureController.Instance;

        cures.OnCureUsed += OnEventCureUsed;
        cures.OnCureUpgraded += OnEventCureUpgraded;
        cures.OnCureRestored += OnEventCureRestored;
        cures.OnHealingStarted += OnHealingStarted;
        cures.OnHealingFinished += OnHealingFinished;

        BuildInitialSlots();
        UpdateVisuals();
    }

    private void BuildInitialSlots()
    {
        if (cures == null || cureUIBase == null) return;
        if (container == null) container = this.transform;

        (current, max) = cures.GetCureInfo();

        // Asegura que slots[0] siempre sea el slot base de la escena
        if (slots.Count == 0 || slots[0] == null)
        {
            slots.Clear();
            slots.Add(cureUIBase);
        }
        else
        {
            // Limpia SOLO los clones (índices >= 1)
            for (int i = slots.Count - 1; i >= 1; i--)
            {
                if (slots[i] != null) Destroy(slots[i].gameObject);
                slots.RemoveAt(i);
            }
        }

        // Ajusta cantidad de clones a (max - 1)
        for (int i = 1; i < max; i++)
        {
            var slot = Instantiate(cureUIBase, container);
            slot.gameObject.SetActive(true);
            slots.Add(slot);
        }

        healingIndex = -1;
        isHealing = false;

        // Asegura que el base esté visible
        slots[0].gameObject.SetActive(true);
    }

    private void UpdateVisuals()
    {
        (current, max) = cures.GetCureInfo();
        if (max != slots.Count) BuildInitialSlots();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;

            bool active = i < current;

            if (isHealing && i == healingIndex)
            {
                // el que se consume: visible y en Healing
                slots[i].SetActiveVisual(true);
                slots[i].SetState(CureStates.Healing);
            }
            else if (isHealing && active)
            {
                // los demás disponibles: parpadean
                slots[i].SetActiveVisual(true);
                slots[i].SetState(CureStates.Ready, blink: true);
            }
            else
            {
                // normal
                slots[i].SetActiveVisual(active);
                slots[i].SetState(active ? CureStates.Ready : CureStates.NoCharges);
            }
        }
    }

    private void OnEventCureUsed(int remaining, int maxQ)
    {
        current = remaining;
        max = maxQ;

        healingIndex = Mathf.Clamp(current, 0, slots.Count - 1);
        isHealing = true;

        UpdateVisuals();
    }

    private void OnHealingStarted(float duration)
    {
        isHealing = true;
        UpdateVisuals();
    }

    private void OnHealingFinished()
    {
        isHealing = false;
        healingIndex = -1;
        UpdateVisuals();
    }

    private void OnEventCureUpgraded(int newMax)
    {
        BuildInitialSlots();
        UpdateVisuals();
    }

    private void OnEventCureRestored(int restored)
    {
        isHealing = false;
        healingIndex = -1;
        UpdateVisuals();
    }
}
