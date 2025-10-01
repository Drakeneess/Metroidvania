using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmilHouse : StoryEvent
{
    [Header("Puertas / Triggers")]
    [SerializeField] private TriggerEvent emilRoomDoor; // puerta para salir de la HABITACIÓN
    [SerializeField] private TriggerEvent houseDoor;    // puerta para salir de la CASA

    [Header("Interacciones")]
    [SerializeField] private StoryInteractable mirrorInteraction;
    [SerializeField] private GameObject mirrorController;
    [SerializeField] private GameObject mirrorMock;

    [Header("Bloqueos físicos (colliders)")]
    [SerializeField] private Collider roomDoorBlocker;   // muro/box collider que bloquea salir de la habitación

    [Header("Colapso")]
    [SerializeField] private GameObject collapsedHouse;      // anim con Trigger "Collapse"
    [SerializeField] private GameObject collapseEffects;  // partículas/sonidos
    [SerializeField] private float collapseDelay = 3f;    // espera antes de cambiar de escena

    private bool hasMirror = false;
    private bool collapseStarted = false;
    private bool collapseFinished = false;

    private void OnEnable()
    {
        if (mirrorInteraction) mirrorInteraction.onInteract.AddListener(OnMirrorTaken);
        if (emilRoomDoor) emilRoomDoor.onPlayerEnter.AddListener(OnExitRoom);
        if (houseDoor) houseDoor.onPlayerEnter.AddListener(OnExitHouse);
    }

    private void OnDisable()
    {
        if (mirrorInteraction) mirrorInteraction.onInteract.RemoveListener(OnMirrorTaken);
        if (emilRoomDoor) emilRoomDoor.onPlayerEnter.RemoveListener(OnExitRoom);
        if (houseDoor) houseDoor.onPlayerEnter.RemoveListener(OnExitHouse);
    }

    protected override void Start()
    {
        eventID = 1; // ID único del evento
        base.Start(); // La lógica base decide si ir a InitPreEvent o OnEventCompleted
    }

    // ================================================================
    // INICIALIZACIÓN DEL EVENTO (antes de completarse)
    // ================================================================
    protected override void InitPreEvent()
    {
        if (roomDoorBlocker) roomDoorBlocker.enabled = true;
        if (collapseEffects) collapseEffects.SetActive(false);
        if (mirrorController) mirrorController.SetActive(false);
        if (mirrorMock) mirrorMock.SetActive(true);

        VolumeManager.Instance.ActivateProfile(0);
    }

    // ================================================================
    // FLUJO DEL EVENTO
    // ================================================================

    // 1) Toma del espejo
    public void OnMirrorTaken()
    {
        hasMirror = true;
        if (mirrorController) mirrorController.SetActive(true);
        if (mirrorMock) mirrorMock.SetActive(false);

        if (roomDoorBlocker) roomDoorBlocker.enabled = false;
    }

    // 2) Al salir de la habitación → inicia el derrumbe
    private void OnExitRoom()
    {
        if (!hasMirror)
        {
            ShowCantLeaveThought();
            return;
        }

        if (!collapseStarted)
        {
            collapseStarted = true;
            StartCollapse();
        }
    }

    // 3) Al salir de la casa → concluye el derrumbe
    public void OnExitHouse()
    {
        if (!hasMirror)
        {
            ShowCantLeaveThought();
            return;
        }

        if (collapseStarted && !collapseFinished)
        {
            collapseFinished = true;
            StartCoroutine(FinishCollapseSequence());
        }
    }

    private void StartCollapse()
    {
        if (CameraShaker.Instance != null)
            CameraShaker.Instance.StartShake(0.2f);

        if (collapseEffects) collapseEffects.SetActive(true);
    }

    private IEnumerator FinishCollapseSequence()
    {
        FadeController.Instance?.FadeIn();
        GameMenuController.CurrentMode = GameMode.ToolMenu;
        VolumeManager.Instance.ActivateProfile(1);

        yield return new WaitForSeconds(collapseDelay);

        if (CameraShaker.Instance != null)
            CameraShaker.Instance.StopShake();

        if (collapsedHouse) collapsedHouse.SetActive(true);

        // Se guarda el progreso
        SaveDataController.Instance.saveData.currentStoryEvent = eventID;
        Debug.Log("El derrumbe concluyó y Emil salió de la casa.");

        FadeController.Instance?.FadeOut();
        GameMenuController.CurrentMode = GameMode.Game;

        // Ya no necesitamos este controlador
        gameObject.SetActive(false);
    }

    // ================================================================
    // ESTADO FINAL DEL EVENTO (cuando ya estaba completado al cargar)
    // ================================================================
    protected override void OnEventCompleted()
    {
        VolumeManager.Instance.ActivateProfile(1);

        if (collapsedHouse) collapsedHouse.SetActive(true);
        if (mirrorController) mirrorController.SetActive(true);
        if (mirrorMock) mirrorMock.SetActive(false);

        // Desactivamos el controlador para que no repita lógica
        gameObject.SetActive(false);
    }

    private void ShowCantLeaveThought()
    {
        if (ThoughtSystem.Instance != null)
            ThoughtSystem.Instance.Show("1");
    }
}
