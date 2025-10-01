using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animation/Player State Registry", fileName = "PlayerAnimationStateRegistry")]
public class AnimationStateRegistryAsset : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public PlayerAnimationState state;
        public AnimationStateType type = AnimationStateType.Persistent;
        public int priority = 0;
        [Tooltip("Para transitorios. Si es 0, revertirá por Animation Event (NotifyTransientFinished).")]
        public float duration = 0f;
    }

    [SerializeField] private List<Entry> states = new();

    private Dictionary<PlayerAnimationState, AnimationStateMetadata> _map;

    public AnimationStateMetadata Get(PlayerAnimationState s)
    {
        if (_map == null) Build();
        return _map.TryGetValue(s, out var m)
            ? m
            : new AnimationStateMetadata(PlayerAnimationState.Idle, AnimationStateType.Persistent, 0, 0f);
    }

    [ContextMenu("Rebuild Map")]
    public void Build()
    {
        _map = new Dictionary<PlayerAnimationState, AnimationStateMetadata>();
        foreach (var e in states)
            _map[e.state] = new AnimationStateMetadata(e.state, e.type, e.priority, e.duration);
    }

    private void OnValidate() => Build();
}
