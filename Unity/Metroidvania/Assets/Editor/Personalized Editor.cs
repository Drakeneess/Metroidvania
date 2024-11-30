using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Interactable))]
public class PersonalizedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Obtener referencia al objeto Interactable
        Interactable interactable = (Interactable)target;

        // Dibujar todos los campos, pero omitimos timeToAction.
        SerializedProperty timeToActionProperty = serializedObject.FindProperty("timeToAction");

        // Mostrar todos los campos en el Inspector (menos timeToAction)
        DrawPropertiesExcluding(serializedObject, "timeToAction");

        // Solo mostrar `timeToAction` si el tipo de interacción es "Hold"
        if (interactable.interactionType == Interactable.InteractionType.Hold)
        {
            // Mostrar el campo `timeToAction` en el Inspector solo si interactionType es "Hold"
            EditorGUILayout.PropertyField(timeToActionProperty);
        }

        // Guardar los cambios en el objeto
        serializedObject.ApplyModifiedProperties();
    }
}