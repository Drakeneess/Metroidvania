using System.Collections;
using UnityEngine;

/// <summary>
/// FSM para animaciones del Player — Política B1:
/// - Idle/Walk son bases y NO se apilan como persistentes.
/// - Persistentes SOLO vuelven si siguen activos externamente.
/// - Al finalizar transitorio -> siempre reevaluar base por input.
/// </summary>
public class AnimStateMachine
{
    private readonly MonoBehaviour host;
    private readonly AnimationStateRegistryAsset registry;
    private readonly System.Func<PlayerAnimationState, IAnimState> getState;
    private readonly System.Func<PlayerAnimationState> resolveBase;

    private Coroutine revertRoutine;
    private bool waitingForEvent;

    // Persistente actual (solo 1 permitido)
    public PlayerAnimationState? ActivePersistent { get; private set; } = null;

    public PlayerAnimationState Current { get; private set; }

    private const bool LOG = true; // cambia a true para debug

    public AnimStateMachine(
        MonoBehaviour host,
        AnimationStateRegistryAsset registry,
        System.Func<PlayerAnimationState, IAnimState> getState,
        System.Func<PlayerAnimationState> resolveBase)
    {
        this.host = host;
        this.registry = registry;
        this.getState = getState;
        this.resolveBase = resolveBase;

        Current = resolveBase();
    }

    public bool HasAnyPersistentActive => ActivePersistent.HasValue;

    public void Request(PlayerAnimationState next, bool force = false)
    {
        var newMeta = registry.Get(next);
        var curMeta = registry.Get(Current);

        if (!force && next == Current) return;

        bool bothTransient = newMeta.Type == AnimationStateType.Transient && curMeta.Type == AnimationStateType.Transient;
        if (!force && bothTransient && newMeta.Priority < curMeta.Priority)
            return;

        getState(Current)?.Exit();
        Current = next;
        getState(Current)?.Enter();

        if (LOG) Debug.Log($"[FSM] {curMeta.State} -> {next}");
        HandleTransientRevert(newMeta);
    }

    /// <summary>
    /// ON/OFF de persistentes. Idle/Walk quedan excluidos.
    /// </summary>
    public void TogglePersistent(PlayerAnimationState state, bool enable)
    {
        var meta = registry.Get(state);
        if (meta.Type != AnimationStateType.Persistent)
        {
            if (LOG) Debug.LogWarning($"[FSM] {state} no es persistente.");
            return;
        }

        if (enable)
        {
            // Salir del estado actual (Idle/Walk o lo que esté) para que no deje flags activos
            getState(Current)?.Exit();

            // Si había otro persistente con menor prioridad, lo apagamos
            if (ActivePersistent.HasValue)
            {
                var activeMeta = registry.Get(ActivePersistent.Value);
                if (meta.Priority < activeMeta.Priority)
                {
                    // Volvemos a entrar al estado actual porque no cambiaremos a 'state'
                    getState(Current)?.Enter();
                    return;
                }

                // Salimos del persistente previo
                getState(ActivePersistent.Value)?.Exit();
            }

            ActivePersistent = state;
            if (LOG) Debug.Log($"[FSM] Persistent ON → {state}");
            getState(state)?.Enter();
            Current = state; // el persistente pasa a ser Current
        }
        else
        {
            if (ActivePersistent != state) return;

            if (LOG) Debug.Log($"[FSM] Persistent OFF → {state}");
            getState(state)?.Exit();
            ActivePersistent = null;

            // Vuelve a la base (Idle/Walk por input actual)
            var baseState = resolveBase();
            Request(baseState, force: true);
        }
    }


    private void HandleTransientRevert(AnimationStateMetadata meta)
    {
        if (meta.Type == AnimationStateType.Persistent)
        {
            waitingForEvent = false;
            if (revertRoutine != null) host.StopCoroutine(revertRoutine);
            revertRoutine = null;
            return;
        }

        if (meta.Duration > 0f)
        {
            waitingForEvent = false;
            if (revertRoutine != null) host.StopCoroutine(revertRoutine);
            revertRoutine = host.StartCoroutine(RevertAfter(meta.Duration));
        }
        else
        {
            waitingForEvent = true;
        }

        // Die debe convertirse en el único estado persistente activo
        if (meta.State == PlayerAnimationState.Die)
        {
            waitingForEvent = false;
            if (revertRoutine != null) host.StopCoroutine(revertRoutine);
            revertRoutine = null;
            return; // <- NO revertir, no volver a base
        }
    }

    private IEnumerator RevertAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        RestoreAfterTransient();
    }

    public void NotifyTransientFinished()
    {
        if (!waitingForEvent) return;
        waitingForEvent = false;
        RestoreAfterTransient();
    }

    private void RestoreAfterTransient()
    {
        // 1) Si hay persistente activo, restaurarlo (solo si fue activado externamente y sigue vigente)
        if (ActivePersistent.HasValue)
        {
            var p = ActivePersistent.Value;
            if (LOG) Debug.Log($"[FSM] Back to Persistent → {p}");
            Request(p, force: true);
            return;
        }

        // 2) Volver SIEMPRE a Idle/Walk según input actual
        var baseState = resolveBase();
        if (LOG) Debug.Log($"[FSM] Back to Base → {baseState}");
        Request(baseState, force: true);
    }

    public void ClearAllPersistents()
    {
        if (ActivePersistent.HasValue)
        {
            if (LOG) Debug.Log($"[FSM] Clearing Persistent → {ActivePersistent.Value}");
            getState(ActivePersistent.Value)?.Exit();
            ActivePersistent = null;
        }

        var baseState = resolveBase();
        Request(baseState, force: true);
    }
}
