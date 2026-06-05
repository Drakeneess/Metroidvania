using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputActionTypeExtensions
{
    public static bool IsGameAction(this InputActionType action)
    {
        return action switch
        {
            InputActionType.Movement or
            InputActionType.Run or
            InputActionType.Jump or
            InputActionType.Dash or
            InputActionType.LightAttack or
            InputActionType.HeavyAttack or
            InputActionType.Block or
            InputActionType.InteractPressed or
            InputActionType.Map or
            InputActionType.Pause or
            InputActionType.Cure => true,
            _ => false
        };
    }

    public static bool IsMenuAction(this InputActionType action)
    {
        return action switch
        {
            InputActionType.Select or
            InputActionType.Back or
            InputActionType.Navigation or
            InputActionType.PAButton => true,
            _ => false
        };
    }

    public static bool IsDialogueAction(this InputActionType action)
    {
        return action switch
        {
            InputActionType.OptionSelect or
            InputActionType.OptionMovement => true,
            _ => false
        };
    }

    public static bool IsToolAction(this InputActionType action)
    {
        return action switch
        {
            InputActionType.ToolSelect or
            InputActionType.RotateTool => true,
            _ => false
        };
    }

    public static bool IsMapAction(this InputActionType action)
    {
        return action switch
        {
            InputActionType.MapNavigation or
            InputActionType.MapZoom or
            InputActionType.CloseMap => true,
            _ => false
        };
    }
}

