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
        [Tooltip("Cuanto mayor, más dominante es al resolver conflictos.")]
        public int priority = 0;

        [Tooltip("Para transitorios. Si es 0, la salida será por Animation Event.")]
        public float duration = 0f;
    }

    [SerializeField] private List<Entry> states = new();

    private Dictionary<PlayerAnimationState, AnimationStateMetadata> map;

    public AnimationStateMetadata Get(PlayerAnimationState s)
    {
        if (map == null) Build();
        return map.TryGetValue(s, out var m)
            ? m
            : new AnimationStateMetadata(PlayerAnimationState.Idle, AnimationStateType.Persistent, 0, 0f);
    }

    [ContextMenu("Rebuild Map")]
    public void Build()
    {
        map = new Dictionary<PlayerAnimationState, AnimationStateMetadata>();
        foreach (var e in states)
        {
            map[e.state] = new AnimationStateMetadata(e.state, e.type, e.priority, e.duration);
        }
    }

    private void OnValidate() => Build();
}
