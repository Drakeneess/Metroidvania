#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AnimationStateRegistryAsset))]
public class AnimationStateRegistryAssetEditor : Editor
{
    // Ajusta si tienes nombres distintos entre Animator y enum
    private static readonly Dictionary<string, PlayerAnimationState> nameToState = new()
    {
        { "Idle", PlayerAnimationState.Idle },
        { "Walk", PlayerAnimationState.Walk },
        { "Jump", PlayerAnimationState.Jumping },
        { "Evade", PlayerAnimationState.Evading },
        { "Cure", PlayerAnimationState.Curing },
        { "Attack", PlayerAnimationState.Attacking },
        { "Block", PlayerAnimationState.Blocking },
        { "Rest", PlayerAnimationState.Rest },
        { "Damage", PlayerAnimationState.TakingDamage }, // submachine/BT que agrupa Damage_0..3
        { "Die", PlayerAnimationState.Die }
    };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var registry = (AnimationStateRegistryAsset)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Auto Fill Durations", EditorStyles.boldLabel);

        var animator = EditorGUILayout.ObjectField("Animator (scene)", FindAnyObjectByType<Animator>(), typeof(Animator), true) as Animator;

        using (new EditorGUI.DisabledScope(animator == null || animator.runtimeAnimatorController == null))
        {
            if (GUILayout.Button("Auto-Fill from AnimatorController"))
            {
                var ctrl = animator.runtimeAnimatorController as AnimatorController;
                if (!ctrl)
                {
                    EditorUtility.DisplayDialog("Error", "El RuntimeAnimatorController no es un AnimatorController.", "Ok");
                    return;
                }

                Undo.RecordObject(registry, "Auto Fill Durations");

                // Recorre todas las layers/maquinas/estados
                foreach (var layer in ctrl.layers)
                {
                    TraverseStateMachine(layer.stateMachine, "", (state, path) =>
                    {
                        if (!nameToState.TryGetValue(state.name, out var enumState)) return;

                        var meta = registry.Get(enumState);
                        if (meta.Type != AnimationStateType.Transient) return;

                        var motion = state.motion;
                        float baseLen = MotionDurationUtil.GetMotionLongestDuration(motion);

                        // Considera multiplicador de velocidad del estado
                        float stateSpeed = state.speed <= 0f ? 1f : state.speed;
                        float effective = baseLen / stateSpeed;

                        // Si este estado es una “familia” (p.ej. Damage con variantes)
                        // usamos el 'longest' por seguridad. Si quieres exactitud, usa Animation Events.
                        SetDurationOnRegistrySerialized(registry, enumState, effective);
                        Debug.Log($"[AutoFill] {enumState} <- {effective:0.00}s (state {state.name} path {path})");
                    });
                }

                EditorUtility.SetDirty(registry);
                AssetDatabase.SaveAssets();
            }
        }

        EditorGUILayout.HelpBox(
            "Para máxima precisión en transitorios variables (Damage/Die/Combos), usa Animation Events (duration=0 en SO) y llama a PlayerAnimationController.AnimEvent_EndTransient() al final del clip.",
            MessageType.Info);
    }

    private static void TraverseStateMachine(AnimatorStateMachine sm, string path, Action<AnimatorState, string> onState)
    {
        foreach (var c in sm.states)
            onState?.Invoke(c.state, path + "/" + c.state.name);

        foreach (var sub in sm.stateMachines)
            TraverseStateMachine(sub.stateMachine, path + "/" + sub.stateMachine.name, onState);
    }

    // Como AnimationStateRegistryAsset expone metadatos vía Get(), aquí hacemos un pequeño setter via SerializedObject
    private static void SetDurationOnRegistrySerialized(AnimationStateRegistryAsset registry, PlayerAnimationState st, float duration)
    {
        var so = new SerializedObject(registry);
        var list = so.FindProperty("states");
        if (list == null) return;

        for (int i = 0; i < list.arraySize; i++)
        {
            var elem = list.GetArrayElementAtIndex(i);
            var stateProp = elem.FindPropertyRelative("state");
            if ((PlayerAnimationState)stateProp.enumValueIndex == st)
            {
                elem.FindPropertyRelative("duration").floatValue = duration;
                so.ApplyModifiedProperties();
                return;
            }
        }
    }
}
#endif
