using System.Collections;
using UnityEngine;

public class MainMenuFlowController : MonoBehaviour
{
    public float beginDelay; // Retraso antes de comenzar la animación
    private MainMenu mainMenu;

    private Coroutine titleCoroutine;
    private bool isSkipping = false; // Control para saber si se está saltando la animación
    private bool buttonsReady = false; // Control para saber si los botones están listos

    private void OnEnable()
    {
        // Suscribirse a los eventos de entrada
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnPressAnyButton += SkipTitleAnimation;
            InputActionController.Instance.OnSelect += ActivateMenuButtons;
        }
    }

    private void OnDisable()
    {
        // Desuscribirse de los eventos
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnPressAnyButton -= SkipTitleAnimation;
            InputActionController.Instance.OnSelect -= ActivateMenuButtons;
        }
    }

    void Start()
    {
        mainMenu = GetComponent<MainMenu>(); // Obtiene la referencia a MainMenu
        if (mainMenu != null)
        {
            StartCoroutine(BeginTitle()); // Comienza la animación del título
        }
    }

    /// <summary>
    /// Comienza la animación del título tras el retraso inicial.
    /// </summary>
    private IEnumerator BeginTitle()
    {
        yield return new WaitForSeconds(beginDelay); // Espera el retraso
        titleCoroutine = StartCoroutine(mainMenu.StartTittleFadeIn()); // Inicia la animación del título
    }

    /// <summary>
    /// Salta la animación del título y la completa de inmediato.
    /// </summary>
    private void SkipTitleAnimation()
    {
        if (isSkipping || titleCoroutine == null) return;

        isSkipping = true;

        // Detiene la corutina actual del título
        StopCoroutine(titleCoroutine);

        // Completa el fade-in del título de inmediato
        mainMenu.CompleteFadeIn(mainMenu.titleContent);

        // Vibración al saltar la animación
        RumbleController.RumblePulse(0.1f, 0.2f, 1f);

        // Espera 2 segundos antes de habilitar los botones
        StartCoroutine(WaitBeforeEnablingButtons());
    }

    /// <summary>
    /// Espera 2 segundos antes de habilitar los botones del menú.
    /// </summary>
    private IEnumerator WaitBeforeEnablingButtons()
    {
        yield return new WaitForSeconds(2f);
        buttonsReady = true;
    }

    /// <summary>
    /// Se llama cuando la animación del título ha terminado.
    /// </summary>
    public void OnTitleAnimationComplete()
    {
        StartCoroutine(WaitBeforeEnablingButtons());
    }

    /// <summary>
    /// Activa los botones del menú si están listos.
    /// </summary>
    private void ActivateMenuButtons()
    {
        if (!buttonsReady) return;

        RumbleController.RumblePulse(0.5f, 1f, 0.2f);
        mainMenu.StartCoroutine(mainMenu.ButtonsFadeIn());
        buttonsReady = false; // Evita que se repita el proceso
    }
}
