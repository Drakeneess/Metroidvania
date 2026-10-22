using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// Rebuilds the old multi-camera composition as an explicit URP camera stack.
///
/// The project historically relied on Camera.depth + Clear Flags to compose the
/// gameplay camera with a second screen camera. URP requires that screen-space
/// composition to be expressed as Base + Overlay cameras. RenderTexture cameras
/// are intentionally left as Base cameras because they render independent views.
/// </summary>
public static class LegacyCameraStackCompatibility
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ConfigureInitialScene()
    {
        ConfigureCameraStack();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ConfigureCameraStack();
    }

    private static void ConfigureCameraStack()
    {
        Camera baseCamera = Camera.main;
        if (baseCamera == null || baseCamera.targetTexture != null)
            return;

        UniversalAdditionalCameraData baseData = baseCamera.GetUniversalAdditionalCameraData();
        baseData.renderType = CameraRenderType.Base;

        List<Camera> stack = baseData.cameraStack;
        if (stack == null)
            return;

        stack.Clear();

        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera camera in cameras)
        {
            if (camera == null || camera == baseCamera || !camera.isActiveAndEnabled)
                continue;

            // Mirror/security/etc. cameras render to their own textures and must
            // remain independent Base cameras.
            if (camera.targetTexture != null)
            {
                camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
                continue;
            }

            // Preserve the old rendering order: only cameras that rendered after
            // the Main Camera become overlays.
            if (camera.depth <= baseCamera.depth)
                continue;

            UniversalAdditionalCameraData overlayData = camera.GetUniversalAdditionalCameraData();
            overlayData.renderType = CameraRenderType.Overlay;
            overlayData.clearDepth = camera.clearFlags == CameraClearFlags.Depth;
            stack.Add(camera);
        }
    }
}
