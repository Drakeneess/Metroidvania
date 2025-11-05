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
    public RectTransform Content => content;

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
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true); // 🔹 garantizar activación

        player = p;
        checkpointMenu = cmc;
        isDeployed = true;
        areOptionsDeployed = true;

        menuButtons?.RefreshButtons();
    }

    private void OnSelectCheckpoint(Checkpoint cp)
    {
        FadeController.Instance.FadeIn(1);
        Debug.Log($"[FastTravel] Selected checkpoint ID: {cp.CheckpointID} ({cp.name})");
        Debug.Log($"[FastTravel] Checkpoint world position: {cp.transform.position}");

        SaveDataController.Instance.saveData.lastCheckpointIndex = cp.CheckpointID;
        SaveDataController.SaveData();

        player.LastCheckpoint = cp;
        player.SetOnCheckpointPosition();

        Debug.Log($"[FastTravel] Player teleported to: {player.transform.position}");
        FadeController.Instance.FadeOut(1);

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
