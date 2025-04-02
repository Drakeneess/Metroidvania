using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenu : Menu
{
    // Elementos de la interfaz del menú
    public GameObject[] menuButtons;
    public GameObject[] titleContent;
    public GameObject courtine; // Cortina de fondo del menú
    public float fadeDurationTitle = 1.0f; // Duración del fade-in del título
    public float fadeDurationButtons = 1.0f; // Duración del fade-in de los botones
    public float verticalSpacing = 50f; // Espaciado vertical entre botones
    public float horizontalOffset = 30f; // Desplazamiento horizontal de los botones

    // Función de inicio, inicializa el estado del menú
    private void Start()
    {
        GameMenuController.CurrentMode = GameMode.Menu;

        // Inicializa los títulos y los botones
        InitializeElements(titleContent, Vector3.zero);
        InitializeElements(menuButtons, new Vector3(horizontalOffset, -verticalSpacing, 0));
    }

    /// <summary>
    /// Inicializa los elementos (opacidad y posición).
    /// </summary>
    private void InitializeElements(GameObject[] elements, Vector3 offsetStep)
    {
        int position=0;
        for (int i = 0; i < elements.Length; i++)
        {
            GameObject element = elements[i];
            if (element != null)
            {
                CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
                canvasGroup.alpha = 0f; // Inicializa la opacidad a 0

                RectTransform rectTransform = element.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    position=NotDeployButtonContinue(element)?position-1:position;
                    rectTransform.localPosition += offsetStep * position; // Aplica un desplazamiento según el índice
                }

                position++;
                element.SetActive(false);
            }
        }
    }


    /// <summary>
    /// Obtiene o agrega un CanvasGroup a un objeto.
    /// </summary>
    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>(); // Si no tiene un CanvasGroup, lo agrega
        }
        return canvasGroup;
    }

    /// <summary>
    /// Animación de fade-in para el título.
    /// </summary>
    private IEnumerator TitleFadeIn()
    {
        // Recorre todos los elementos del título
        for (int i = 0; i < titleContent.Length; i++)
        {
            
            GameObject titleItem = titleContent[i];
            titleItem.SetActive(true);
            float duration = (i == titleContent.Length - 1) ? fadeDurationTitle * 1.5f : fadeDurationTitle; // Aumenta la duración del último ítem

            // Realiza el fade-in
            yield return StartCoroutine(FadeElement(titleItem, duration, 1f));

            // Vibración de la controladora (feedback)
            RumbleController.RumblePulse(0.05f, 0.1f, 0.7f);
            yield return new WaitForSeconds(0.1f); // Pausa breve entre elementos
        }

        // Cuando la animación del título termine, se notifica al controlador de flujo
        MainMenuFlowController flowController = GetComponent<MainMenuFlowController>();
        if (flowController != null)
        {
            flowController.OnTitleAnimationComplete();
        }
    }

    /// <summary>
    /// Inicia la animación de fade-in del título.
    /// </summary>
    public IEnumerator StartTittleFadeIn()
    {
        yield return StartCoroutine(TitleFadeIn());
    }

    /// <summary>
    /// Animación de fade-in para los botones del menú.
    /// </summary>
    public IEnumerator ButtonsFadeIn()
    {
        // Desaparece el último elemento del título antes de mostrar los botones
        yield return StartCoroutine(FadeElement(titleContent[^1], 2, 0));

        // Despliega los botones
        foreach (GameObject button in menuButtons)
        {
            if(NotDeployButtonContinue(button)){
                continue;
            }
            else{
                button.SetActive(true);
                yield return StartCoroutine(FadeElement(button, fadeDurationButtons, 1f));

                // Vibración al interactuar con un botón
                RumbleController.RumblePulse(0.01f, 0.07f, 0.4f);
            }
            }

        if(SaveDataController.AreSavedData()){
            // Hace que la cortina desaparezca después de que los botones se muestren
            StartCoroutine(CourtineFadeOut());
        }

        // Marca que los botones han sido desplegados
        areOptionsDeployed = true;
    }

    /// <summary>
    /// Animación de fade-out para la cortina del fondo.
    /// </summary>
    private IEnumerator CourtineFadeOut()
    {
        yield return StartCoroutine(FadeElement(courtine, 2f, 0f)); // Desaparece la cortina
    }

    /// <summary>
    /// Animación de fade para un elemento con control de duración.
    /// </summary>
    private IEnumerator FadeElement(GameObject element, float duration, float targetAlpha)
    {
        CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
        float startAlpha = canvasGroup.alpha; // Almacena el valor de opacidad inicial
        float elapsedTime = 0f;

        // Aplica la interpolación de fade durante la duración especificada
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha; // Asegura que el valor final se aplique
    }

    /// <summary>
    /// Completa instantáneamente el fade-in de los elementos.
    /// </summary>
    public void CompleteFadeIn(GameObject[] elements)
    {
        StopAllCoroutines(); // Detiene todas las corutinas en ejecución
        foreach (GameObject element in elements)
        {
            if (element != null)
            {
                element.SetActive(true);
                CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);
                canvasGroup.alpha = 1f; // Asegura que los elementos estén completamente visibles
            }
        }
    }

    private bool NotDeployButtonContinue(GameObject element){
        return element==menuButtons[0] && !SaveDataController.AreSavedData();
    }
}
