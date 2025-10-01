using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CureUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image slotImage;   // fondo/marco
    [SerializeField] private Image chargeImage; // usar Filled

    private CureStates currentState;
    private Vector3 originalChargeScale;
    private Coroutine currentRoutine;

    void Awake()
    {
        if (slotImage != null) slotImage.enabled = true;

        if (chargeImage != null)
        {
            // 🔧 Normaliza escala si vino en 0
            var rt = chargeImage.rectTransform;
            if (IsZeroish(rt.localScale))
                rt.localScale = Vector3.one;

            originalChargeScale = rt.localScale;
            chargeImage.fillAmount = 1f;
        }
        else
        {
            originalChargeScale = Vector3.one;
        }
    }

    // Llamar siempre que el GO se active (por si algún animator/parent la pisa)
    void OnEnable()
    {
        EnsureSafeScale();
    }

    private void EnsureSafeScale()
    {
        if (chargeImage == null) return;
        var rt = chargeImage.rectTransform;
        if (IsZeroish(rt.localScale))
            rt.localScale = (IsZeroish(originalChargeScale) ? Vector3.one : originalChargeScale);
    }

    private static bool IsZeroish(Vector3 v)
        => Mathf.Approximately(v.x, 0f) || Mathf.Approximately(v.y, 0f) || Mathf.Approximately(v.z, 0f);

    public void SetActiveVisual(bool active)
    {
        if (chargeImage != null)
        {
            EnsureSafeScale();   // 🔒 por si llegara desnormalizada
            chargeImage.enabled = active;
        }
    }

    public void SetState(CureStates state, bool blink = false)
    {
        currentState = state;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        if (chargeImage != null)
        {
            // 🔁 reset visual sin perder la escala segura
            var rt = chargeImage.rectTransform;
            if (IsZeroish(rt.localScale))
                rt.localScale = (IsZeroish(originalChargeScale) ? Vector3.one : originalChargeScale);

            chargeImage.fillAmount = 1f;
            var c = chargeImage.color; c.a = 1f; chargeImage.color = c;
        }

        switch (state)
        {
            case CureStates.Ready:
                TintCharge(1f, 1f, 0f, 1f);
                if (blink) currentRoutine = StartCoroutine(BlinkRoutine());
                break;

            case CureStates.FullHP:
                TintCharge(0.6f, 1f, 0.6f, 1f);
                break;

            case CureStates.NoCharges:
                TintCharge(1f, 1f, 1f, 0f);
                break;

            case CureStates.Healing:
                TintCharge(1f, 1f, 0.5f, 1f);
                float healingDuration = CureController.Instance != null ? CureController.Instance.GetCuringTime() : 0.5f;
                currentRoutine = StartCoroutine(HealingRoutine(healingDuration));
                break;

            case CureStates.Cooldown:
                TintCharge(0.7f, 0.7f, 1f, 1f);
                break;
        }
    }

    private IEnumerator HealingRoutine(float duration)
    {
        float elapsed = 0f;

        if (chargeImage != null)
        {
            EnsureSafeScale(); // 🔒
            chargeImage.fillAmount = 1f;
            var c = chargeImage.color; c.a = 1f; chargeImage.color = c;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (chargeImage != null)
                chargeImage.fillAmount = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        if (chargeImage != null)
        {
            chargeImage.fillAmount = 0f;
            var c = chargeImage.color; c.a = 1f; chargeImage.color = c;
        }

        SetState(CureStates.NoCharges);
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float alpha = 0.5f + Mathf.Sin(Time.time * 6f) * 0.5f;
            var c = chargeImage.color; c.a = alpha; chargeImage.color = c;
            yield return null;
        }
    }

    private void TintCharge(float r, float g, float b, float a)
    {
        if (chargeImage != null) chargeImage.color = new Color(r, g, b, a);
    }
}
