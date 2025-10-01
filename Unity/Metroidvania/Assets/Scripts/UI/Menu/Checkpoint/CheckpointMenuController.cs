using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMenuController : MonoBehaviour
{
    public static CheckpointMenuController Instance { get; private set; }
    [SerializeField] private CheckpointMenu checkpointMenu;
    [SerializeField] MenuContainer checkMenu;
    [SerializeField] FastTravelMenu fastTravelMenu;

    private Player currentPlayer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (checkpointMenu != null) checkpointMenu.gameObject.SetActive(false);
    }

    public void Open(Player player)
    {
        currentPlayer = player;
        GameMenuController.CurrentMode = GameMode.Menu;

        if (checkpointMenu != null)
        {
            checkpointMenu.gameObject.SetActive(true);
            checkpointMenu.Open();
        }
        else
        {
            Debug.LogWarning("[CheckpointMenuController] No hay CheckpointMenu asignado (headless total).");
        }
    }

    void OnEnable()
    {
        MenuTransition.Instance.SetInitial(checkMenu);
    }

    public void Close()
    {
        if (checkpointMenu != null)
            checkpointMenu.Close();

        currentPlayer = null;
    }

    public void DoRest()
    {
        if (currentPlayer != null)
            currentPlayer.RestOnRefugee();
    }

    public void OpenFastTravel()
    {
        Player p = CheckpointsController.Instance.player;
        if (!p) { Debug.LogWarning("[CheckpointMenu] No hay Player."); return; }

        if (fastTravelMenu != null)
        {
            MenuTransition.Instance.SwitchTo(fastTravelMenu.GetComponent<MenuContainer>());
            StartCoroutine(openFastTravelMenu(p));
        }
    }

    private IEnumerator openFastTravelMenu(Player p)
    {
        print(p);
        yield return 0.5f;
        print(p);
        fastTravelMenu.Open(p, checkMenu);
    }
}
