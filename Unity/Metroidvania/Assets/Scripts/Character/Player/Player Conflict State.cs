using System.Collections.Generic;
using UnityEngine;

public class PlayerConflictStateController : MonoBehaviour
{
    public static PlayerConflictStateController Instance;

    // Enemigos actualmente involucrados en el conflicto (y vivos)
    private readonly HashSet<Enemy> _conflictEnemies = new HashSet<Enemy>();

    private bool isInConflict = false;
    public bool IsInConflict => isInConflict;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Enemy.OnEnemyDisabled += OnEnemyDisabled; // baja por muerte/disable
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDisabled -= OnEnemyDisabled;
        _conflictEnemies.Clear();
        if (isInConflict) ForceEndCombat();
    }

    // === API pública ===
    public void RegisterEnemy(Enemy e)
    {
        if (e == null || e.isDead) return;
        if (_conflictEnemies.Add(e))
            print(_conflictEnemies.Count);
            RefreshConflictState(); // pudo pasar de 0 -> 1
    }

    public void UnregisterEnemy(Enemy e)
    {
        if (e == null) return;
        if (_conflictEnemies.Remove(e))
            print(_conflictEnemies.Count);
            RefreshConflictState(); // pudo pasar de 1 -> 0
    }

    // Cuando un Enemy muere / se desactiva
    private void OnEnemyDisabled(Enemy e)
    {
        if (e == null) return;
        if (_conflictEnemies.Remove(e))
            RefreshConflictState();
    }

    private void RefreshConflictState()
    {
        // Limpieza defensiva por si quedó algún null
        _conflictEnemies.RemoveWhere(x => x == null || x.isDead == true);

        if (_conflictEnemies.Count > 0)
            BeginCombat();
        else
            EndCombat();
    }

    // ===== Estado de música/capas =====
    public void BeginCombat()
    {
        if (isInConflict) return;                  // ya estaba en combate
        isInConflict = true;
        // Activa capa de música de combate
        if (MusicController.Instance != null && MusicController.Instance.GetCurrentTheme() != null)
            MusicController.Instance.GetCurrentTheme().SetLayerActive(1, true);
    }

    public void EndCombat()
    {
        if (!isInConflict) return;                 // <-- FIX del bug: antes salía si SÍ estaba en conflicto
        isInConflict = false;
        if (MusicController.Instance != null && MusicController.Instance.GetCurrentTheme() != null)
            MusicController.Instance.GetCurrentTheme().SetLayerActive(1, false);
    }

    // Si necesitas cerrar combate sin validar lista (escena/unload)
    private void ForceEndCombat()
    {
        isInConflict = false;
        if (MusicController.Instance != null && MusicController.Instance.GetCurrentTheme() != null)
            MusicController.Instance.GetCurrentTheme().SetLayerActive(1, false);
    }
}
