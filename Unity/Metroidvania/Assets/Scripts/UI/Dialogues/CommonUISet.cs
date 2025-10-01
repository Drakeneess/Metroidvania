using UnityEngine;

/// <summary>
/// Activa/desactiva los elementos comunes, de diálogo o de pensamiento
/// en función de qué panel se quiera mostrar.
/// </summary>
public class CommonUISet : MonoBehaviour
{
    [Header("Roots específicos")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private GameObject thoughtRoot;

    /// <summary> Activa todo para diálogos. </summary>
    public void ActivateForDialogue()
    {
        gameObject.SetActive(true); // comunes
        if (dialogueRoot) dialogueRoot.SetActive(true);
        if (thoughtRoot) thoughtRoot.SetActive(false);
    }

    /// <summary> Activa todo para pensamientos. </summary>
    public void ActivateForThought()
    {
        gameObject.SetActive(true); // comunes
        if (dialogueRoot) dialogueRoot.SetActive(false);
        if (thoughtRoot) thoughtRoot.SetActive(true);
    }

    /// <summary> Desactiva absolutamente todo. </summary>
    public void DeactivateAll()
    {
        gameObject.SetActive(false);
        if (dialogueRoot) dialogueRoot.SetActive(false);
        if (thoughtRoot) thoughtRoot.SetActive(false);
    }
}
