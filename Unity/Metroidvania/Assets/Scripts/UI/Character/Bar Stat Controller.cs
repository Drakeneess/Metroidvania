using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour {
    public HealthType healthType;
    public Image mainBar;
    public Image delayedBar;

    protected Character target;
    protected float maxValue;
    protected float currentValue;
    protected float delayedValue;

    [Header("Health Bar Update Speeds")]
    public float mainBarUpdateSpeed = 0.05f; // velocidad barra principal
    [Range(0f, 1f)]
    public float lerpSpeed = 0.02f;          // velocidad de transición "lerp"
    public float delayedBarSpeed = 0.01f;    // velocidad barra retrasada

    protected virtual void Start() {
        if (target != null) {
            maxValue = target.Health.GetMax(healthType);
            currentValue = target.Health.Get(healthType);
            delayedValue = currentValue;
        }
    }

    protected virtual void Update() {
        if (target == null) return;

        // actualizar valores actuales
        currentValue = target.Health.Get(healthType);
        if (delayedValue < currentValue) {
            delayedValue = currentValue; // reset cuando curas
        }

        // barra principal (rápida)
        mainBar.fillAmount = Mathf.MoveTowards(mainBar.fillAmount, currentValue / maxValue, mainBarUpdateSpeed);

        // barra retrasada (suave, "delay")
        delayedValue = Mathf.MoveTowards(delayedValue, currentValue, delayedBarSpeed);
        delayedBar.fillAmount = delayedValue / maxValue;
    }
}
