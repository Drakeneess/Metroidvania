using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtUIController : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image thoughtIcon;         // ícono de pensamiento (ej. bombilla o nube)
    public Image nextArrow;           // flecha o puntitos
    public Color pressColor = Color.gray;
    public float pressEffectDuration = 0.1f;

    private Color originalIconColor;
    private Color originalArrowColor;

    private void Awake()
    {
        if (thoughtIcon != null) originalIconColor = thoughtIcon.color;
        if (nextArrow != null) originalArrowColor = nextArrow.color;
    }

    private void OnEnable()
    {
        SetOriginalState();

        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered += HandleInput;
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered -= HandleInput;
    }

    private void HandleInput(InputActionType actionName)
    {
        if (actionName == InputActionType.Select) // ⚡ define esta acción en tu InputActionController
        {
            if (ThoughtSystem.Instance != null)
            {
                // 🔹 Si hay un thought activo, lo cierra de inmediato
                StartCoroutine(ButtonPressEffect());
                ForceCloseOrSkip();
            }
        }
    }

    private void ForceCloseOrSkip()
    {
        // Aquí decides si quieres que:
        // 1) Si el typewriter está en curso → terminar de escribir al instante
        // 2) Si ya terminó → cerrar inmediatamente

        // Para eso añadimos un método público en ThoughtSystem
        //ThoughtSystem.Instance.ForceCompleteOrHide();
    }

    private IEnumerator ButtonPressEffect()
    {
        SetPressState();
        yield return new WaitForSeconds(pressEffectDuration);
        SetOriginalState();
    }

    private void SetPressState()
    {
        if (thoughtIcon != null) thoughtIcon.color = pressColor;
        if (nextArrow != null) nextArrow.color = pressColor;
    }

    private void SetOriginalState()
    {
        if (thoughtIcon != null) thoughtIcon.color = originalIconColor;
        if (nextArrow != null) nextArrow.color = originalArrowColor;
    }
}
