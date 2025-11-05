using System;
using UnityEngine;

public class CharacterMap : MonoBehaviour
{
    [SerializeField] private MapUIController mapUIController;
    [SerializeField] private float reopenCooldown = 0.15f; // 150 ms de debounce

    private float _blockUntil = -1f;

    void OnEnable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered += HandleMapInput;

        MapEvents.OnMapClosed += OnMapClosed; // 🔹 escuchar cierre
    }

    void OnDisable()
    {
        if (InputActionController.Instance != null)
            InputActionController.Instance.OnActionTriggered -= HandleMapInput;

        MapEvents.OnMapClosed -= OnMapClosed;
    }

    private void OnMapClosed()
    {
        // Bloquea re-apertura inmediata mientras el botón sigue down
        _blockUntil = Time.unscaledTime + reopenCooldown;
    }

    private void HandleMapInput(string actionName)
    {
        if (actionName != "Map") return;
        if (mapUIController == null) return;

        // 🔒 No abrir si ya está abierto
        if (mapUIController.gameObject.activeInHierarchy) return;

        // ⏱️ Debounce tras cierr
        if (Time.unscaledTime < _blockUntil) return;

        mapUIController.gameObject.SetActive(true);
        PlayerActionLogger.Instance.Log("Open Map");
    }
}

public static class MapEvents
{
    public static Action OnMapClosed;
}
