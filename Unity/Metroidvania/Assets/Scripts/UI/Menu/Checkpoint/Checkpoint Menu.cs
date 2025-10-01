using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenu : Menu
{
    protected override void Start()
    {
        // Mantiene la semántica del modo menú del base
        base.Start();
    }

    public void Open()
    {
        // Sin fades; solo flags para que quien consulte el estado no reviente
        isDeployed = true;
        areOptionsDeployed = true; // true para no bloquear lógicas que lo consulten
        gameObject.SetActive(true);
    }

    public void Close()
    {
        GameMenuController.CurrentMode = GameMode.Game;
        PlayerAnimationController.SetWalkState(false, true);

        areOptionsDeployed = false;
        isDeployed = false;
        gameObject.SetActive(false);

        var extras = new List<string> { "Menu: Checkpoint", "Action: Exit" };
        PlayerActionLogger.Instance.Log("EndInteraction", extras);
    }
}
