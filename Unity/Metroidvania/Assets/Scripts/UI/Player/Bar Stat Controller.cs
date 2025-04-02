using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BarStatController : MonoBehaviour {
    public HealthType healthType;
    public Image mainBar;
    public Image delayedBar;

    private Player player;
    private float maxValue;
    private float currentValue = 0.5f;
    private float delayedValue = 0.5f;

    [Header("Health Bar Update Speeds")]
    [Tooltip("Speed of the main health bar update (lower values make it slower).")]
    public float mainBarUpdateSpeed = 0.05f;  // Control the speed of the main health bar (0.05 for slower)

    [Tooltip("Speed of the delayed health bar update (lower values make it slower).")]
    [Range(0f, 1f)]  // Adjust this to a very low value to make it slower
    public float lerpSpeed = 0.02f;  // Speed of the delayed bar update (slow speed)

    // Speed of the delayed bar update (constant speed, no slowing down)
    public float delayedBarSpeed = 0.01f;  // Speed of the delayed bar, adjust for constant speed

    void Start() {
        player = FindObjectOfType<Player>(); // Find the Player object
        maxValue = player.GetMaxHealth(healthType);
        currentValue = player.GetCurrentHealth(healthType);
        delayedValue = currentValue; // Start with the same value as the main bar
    }

    void Update() {
        if(delayedValue < currentValue){
            delayedValue = currentValue;
        }
        // Update the main health bar with a slower update speed
        currentValue = player.GetCurrentHealth(healthType);
        mainBar.fillAmount = Mathf.MoveTowards(mainBar.fillAmount, currentValue / maxValue, mainBarUpdateSpeed);

        // Use MoveTowards for the delayed bar to ensure a constant transition speed
        delayedValue = Mathf.MoveTowards(delayedValue, currentValue, delayedBarSpeed);
        delayedBar.fillAmount = delayedValue / maxValue;

    }
}
