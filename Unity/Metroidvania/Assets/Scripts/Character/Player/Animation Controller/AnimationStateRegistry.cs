using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationStateRegistry
{
    private static readonly Dictionary<PlayerAnimationState, AnimationStateMetadata> stateMap = new()
    {
        { PlayerAnimationState.Idle, new AnimationStateMetadata(PlayerAnimationState.Idle, AnimationStateType.Persistent, 10) },
        { PlayerAnimationState.Walk, new AnimationStateMetadata(PlayerAnimationState.Walk, AnimationStateType.Persistent, 20) },
        { PlayerAnimationState.Attacking, new AnimationStateMetadata(PlayerAnimationState.Attacking, AnimationStateType.Transient,60, 0.5f) },
        { PlayerAnimationState.Blocking, new AnimationStateMetadata(PlayerAnimationState.Blocking, AnimationStateType.Persistent, 70) },
        { PlayerAnimationState.Jumping, new AnimationStateMetadata(PlayerAnimationState.Jumping, AnimationStateType.Transient, 80, 0.6f) },
        { PlayerAnimationState.TakingDamage, new AnimationStateMetadata(PlayerAnimationState.TakingDamage, AnimationStateType.Transient, 90, 0.7f) },
        { PlayerAnimationState.Die, new AnimationStateMetadata(PlayerAnimationState.Die, AnimationStateType.Transient, 100, 2f) },
        { PlayerAnimationState.Rest, new AnimationStateMetadata(PlayerAnimationState.Rest, AnimationStateType.Persistent) },
    };

    public static AnimationStateMetadata Get(PlayerAnimationState state)
    {
        return stateMap[state];
    }
}

