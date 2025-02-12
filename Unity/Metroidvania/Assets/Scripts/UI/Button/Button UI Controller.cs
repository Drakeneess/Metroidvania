using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUIController : MonoBehaviour
{
    [Header("Game Icons")]
    public List<ControlSchemeIcons> controlSchemeIcons; // Lista de esquemas con sus íconos
    private Dictionary<ActionType, Sprite[]> iconsPerAction;

    private ButtonUI[] buttonsUI;

    private void Awake()
    {
        buttonsUI = FindObjectsOfType<ButtonUI>();
        InitializeIconsDictionary();
    }
    private void Start() {
        UpdateButtonUI(0);
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
        iconsPerAction = new Dictionary<ActionType, Sprite[]>();
        foreach (var scheme in controlSchemeIcons)
        {
            iconsPerAction[scheme.actionType] = scheme.icons;
        }
    }

    private void UpdateButtonUI(int newScheme)
    {
        foreach (ButtonUI buttonUI in buttonsUI)
        {
            if (iconsPerAction.TryGetValue(buttonUI.GetActionType(), out Sprite[] icons) && icons.Length > newScheme)
            {
                buttonUI.SetButtonIcon(icons[newScheme]);
            }
        }
    }
}

[System.Serializable]
public class ControlSchemeIcons
{
    public ActionType actionType;
    public Sprite[] icons; // Íconos para cada esquema de control
}

public enum ActionType
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
