using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputActionType
{
    None,

    // ---- Game ----
    Jump,
    Dash,
    LightAttack,
    HeavyAttack,
    Block,
    Run,
    ChangeWeapon,
    InteractPressed,
    OnInteractHold,
    Cure,
    Map,
    Pause,

    // ---- Dialogue ----
    OptionSelect,
    OptionMovement,

    // ---- Menu ----
    Select,
    Back,
    PAButton,
    Navigation,

    // ---- Map ----
    CloseMap,
    MapZoom,
    MapNavigation,

    // ---- Tool Menu ----
    RotateTool,
    ToolSelect,

    // ---- Movement ----
    Movement
}

