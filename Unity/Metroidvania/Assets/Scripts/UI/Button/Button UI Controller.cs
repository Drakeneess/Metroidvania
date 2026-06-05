using System.Collections.Generic;
using UnityEngine;

public class ButtonUIController : MonoBehaviour
{
    [Header("Game Icons")]
    public List<ControlSchemeIcons> controlSchemeIcons;
    private static Dictionary<ActionInputType, Sprite[]> iconsPerAction = new Dictionary<ActionInputType, Sprite[]>();

    private static List<ButtonUI> registeredButtons = new List<ButtonUI>();
    private static int actualScheme;

    public static void Register(ButtonUI button)
    {
        if (!registeredButtons.Contains(button))
            registeredButtons.Add(button);
    }

    public static void Unregister(ButtonUI button)
    {
        registeredButtons.Remove(button);  // No es necesario comprobar si está en la lista
    }

    private void Awake()
    {
        InitializeIconsDictionary();
    }

    private void Start()
    {
        int initialScheme = 0;
#if UNITY_ANDROID
        initialScheme = 2;
#endif
        // Actualizamos los botones activos al iniciar
        UpdateButtonUI(initialScheme);
    }

    private void OnEnable()
    {
        InputController.OnControlSchemeChanged += UpdateButtonUI;
    }

    private void OnDisable()
    {
        InputController.OnControlSchemeChanged -= UpdateButtonUI;
    }

    private void InitializeIconsDictionary()
    {
        // Solo inicializar los íconos si no se ha hecho antes
        if (iconsPerAction.Count == 0)
        {
            foreach (var scheme in controlSchemeIcons)
            {
                iconsPerAction[scheme.actionType] = scheme.icons;
            }
        }
    }

    public static void UpdateButtonIcon(ButtonUI buttonUI)
    {
        if (iconsPerAction.TryGetValue(buttonUI.GetActionType(), out Sprite[] icons) && icons.Length > actualScheme)
        {
            buttonUI.SetButtonIcon(icons[actualScheme]);
        }
    }

    public static void UpdateButtonUI(int newScheme)
    {
        actualScheme = newScheme;
        // Solo actualizar los botones activos
        foreach (var buttonUI in registeredButtons)
        {
            UpdateButtonIcon(buttonUI);
        }
    }
}


[System.Serializable]
public class ControlSchemeIcons
{
    public ActionInputType actionType;
    public Sprite[] icons; // Íconos para cada esquema de control
}

public enum ActionInputType
{
    Interaction,
    Movement,
    Jump,
    LightAttack,
    HeavyAttack,
    Block,
    Dash,
    Special,
    Select,
    Cancel,
    Menu
}
