using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenu : Menu
{
    private PlayerAnimationController anim;

    protected override void Start()
    {
        base.Start();

        // Cache del anim del jugador
        var player = FindObjectOfType<Player>();
        if (player != null)
            anim = player.GetComponent<PlayerAnimationController>();
    }

    public void Open()
    {
        isDeployed = true;
        areOptionsDeployed = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        GameMenuController.CurrentMode = GameMode.Game;

        // Antes: PlayerAnimationController.SetWalkState(false, true);

        areOptionsDeployed = false;
        isDeployed = false;
        gameObject.SetActive(false);

        var extras = new List<string> { "Menu: Checkpoint", "Action: Exit" };
        PlayerActionLogger.Instance.Log("EndInteraction", extras);
    }
}
