using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateMetadata
{
    public PlayerAnimationState State { get; }
    public AnimationStateType Type { get; }
    public float Duration { get; }
    public int Priority { get; }

    public AnimationStateMetadata(PlayerAnimationState state, AnimationStateType type, int priority = 0, float duration = 0f)
    {
        State = state;
        Type = type;
        Duration = duration;
        Priority = priority;
    }
}

public enum PlayerAnimationState
{
    Idle,
    Walk,
    Attacking,
    Blocking,
    Jumping,
    Curing,
    TakingDamage,
    Evading,
    Die,
    Rest
}

public enum AnimationStateType
{
    Persistent, // e.g. Idle, Walk, Blocking
    Transient   // e.g. Jump, Attack, TakingDamage
}

