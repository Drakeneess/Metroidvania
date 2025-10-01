using System.Collections;
using UnityEngine;

public class AnimStateMachine
{
    private readonly MonoBehaviour _host;
    private readonly AnimationStateRegistryAsset _registry;
    private readonly System.Func<PlayerAnimationState, IAnimState> _getState;

    private Coroutine _revert;
    private bool _waitingEvent;

    public PlayerAnimationState Current { get; private set; } = PlayerAnimationState.Idle;
    public PlayerAnimationState Previous { get; private set; } = PlayerAnimationState.Idle;

    public AnimStateMachine(
        MonoBehaviour host,
        AnimationStateRegistryAsset registry,
        System.Func<PlayerAnimationState, IAnimState> getState)
    {
        _host = host;
        _registry = registry;
        _getState = getState;
    }

    public void Request(PlayerAnimationState next, bool force = false)
    {
        var newMeta = _registry.Get(next);
        var curMeta = _registry.Get(Current);

        bool same = next == Current;
        if (!force && same) return;
        if (!force && newMeta.Priority < curMeta.Priority) return;

        if (newMeta.Type == AnimationStateType.Transient)
            Previous = Current;

        if (!same)
        {
            _getState(Current)?.Exit();
            Current = next;
            _getState(Current)?.Enter();
        }
        else if (force)
        {
            _getState(Current)?.Exit();
            _getState(Current)?.Enter();
        }

        HandleTransientRevert(newMeta);
    }

    public void NotifyTransientFinished()
    {
        if (_waitingEvent)
        {
            _waitingEvent = false;
            ForceTo(Previous);
        }
    }

    private void HandleTransientRevert(AnimationStateMetadata meta)
    {
        if (_revert != null) { _host.StopCoroutine(_revert); _revert = null; }
        _waitingEvent = false;

        if (meta.Type != AnimationStateType.Transient) return;

        if (meta.Duration > 0f)
            _revert = _host.StartCoroutine(RevertAfter(meta.Duration));
        else
            _waitingEvent = true; // esperar animation event
    }

    private IEnumerator RevertAfter(float secs)
    {
        yield return new WaitForSeconds(secs);
        ForceTo(Previous);
    }

    private void ForceTo(PlayerAnimationState target)
    {
        _getState(Current)?.Exit();
        Current = target;
        _getState(Current)?.Enter();
    }
}
