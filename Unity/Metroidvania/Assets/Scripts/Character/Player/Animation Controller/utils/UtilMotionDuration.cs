#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class MotionDurationUtil
{
    public static float GetMotionLongestDuration(Motion motion)
    {
        if (!motion) return 0f;
        if (motion is AnimationClip clip)
            return clip.length;

        if (motion is BlendTree bt)
        {
            float longest = 0f;
            foreach (var child in bt.children)
                longest = Mathf.Max(longest, GetMotionLongestDuration(child.motion));
            return longest;
        }
        return 0f;
    }
}
#endif
