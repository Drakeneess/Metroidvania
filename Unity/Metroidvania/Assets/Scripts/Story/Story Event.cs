using System.Collections;
using UnityEngine;

public class StoryEvent : MonoBehaviour
{
    protected int eventID = 0;

    /// <summary>
    /// Inicialización principal del evento.
    /// </summary>
    protected virtual void Start()
    {
        if (SaveDataController.AreSavedData() &&
            SaveDataController.Instance.saveData.currentStoryEvent > eventID - 1)
        {
            // 🔹 Ya completado → configura estado final
            OnEventCompleted();
        }
        else
        {
            // 🔹 Aún no completado → inicializa estado previo
            InitPreEvent();
        }
    }

    /// <summary>
    /// Inicialización previa al evento (estado base).
    /// Los hijos deben sobreescribirlo.
    /// </summary>
    protected virtual void InitPreEvent()
    {
        // Aquí no va nada en la base, lo definen los hijos
    }

    /// <summary>
    /// Configuración del estado final del evento ya completado.
    /// </summary>
    protected virtual void OnEventCompleted()
    {
        // Los hijos sobreescriben según lo que quede activo/desactivado
    }
}
