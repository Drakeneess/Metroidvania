using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastTravelMenu : Menu
{
    [Header("UI")]
    [SerializeField] private Button itemPrefab;
    [SerializeField] private RectTransform content;

    private Dictionary<int, Button> checkpointButtons = new(); // ID -> botón
    private Player player;
    private MenuContainer checkpointMenu;

    private FastTravelMenuButtons menuButtons;

    protected override void Start()
    {
        base.Start();
        if (content == null)
            content = GetComponent<RectTransform>();

        menuButtons = GetComponent<FastTravelMenuButtons>();

        gameObject.SetActive(false);
    }

    // 🔹 Llamado cuando un refugio se desbloquea
    public void UnlockCheckpoint(Checkpoint cp)
    {
        if (!checkpointButtons.ContainsKey(cp.CheckpointID))
        {
            var btn = Instantiate(itemPrefab, content);
            btn.gameObject.SetActive(true);

            var txt = btn.GetComponentInChildren<TMPro.TMP_Text>();
            if (txt != null)
                txt.text = string.IsNullOrEmpty(cp.checkpointName)
                    ? $"Checkpoint {cp.CheckpointID}"
                    : cp.checkpointName;

            btn.onClick.AddListener(() => OnSelectCheckpoint(cp));
            checkpointButtons[cp.CheckpointID] = btn;

            // 👉 Avisar al controlador de botones que hay nuevos
            menuButtons?.RefreshButtons();
        }
        else
        {
            checkpointButtons[cp.CheckpointID].gameObject.SetActive(true);
        }
    }

    public void Open(Player p, MenuContainer cmc)
    {
        player = p;
        checkpointMenu = cmc;
        isDeployed = true;
        areOptionsDeployed = true;

        // 🔹 refrescar lista en MenuButtons
        menuButtons?.RefreshButtons();
    }

    private void OnSelectCheckpoint(Checkpoint cp)
    {
        SaveDataController.Instance.saveData.lastCheckpointIndex = cp.CheckpointID;
        SaveDataController.SaveData();

        player.LastCheckpoint = cp;
        player.SetOnCheckpointPosition();

        Close();
    }

    public void Close()
    {
        //checkpointMenu?.SetActive(true);
        //gameObject.SetActive(false);
        MenuTransition.Instance.SwitchTo(checkpointMenu);

        isDeployed = false;
        areOptionsDeployed = false;
    }
}
