using System;
using UnityEngine;

public class ActionCharacterController : MonoBehaviour
{
    public static ActionCharacterController Instance { get; private set; }

    public CharacterAction CurrentAction { get; private set; } = CharacterAction.None;

    public event Action<CharacterAction> OnActionStarted;
    public event Action<CharacterAction> OnActionEnded;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    /// <summary>
    /// Intenta iniciar una acción. Devuelve true si se permite, false si está bloqueada.
    /// </summary>
    public bool TryStartAction(CharacterAction newAction)
    {
        print(CurrentAction);

        if (!CanStartAction(newAction)) return false;

        CurrentAction = newAction;
        OnActionStarted?.Invoke(newAction);
        return true;
    }

    /// <summary>
    /// Finaliza la acción actual (ej: tras terminar animación o coroutine).
    /// </summary>
    public void EndAction(CharacterAction endedAction)
    {
        print(CurrentAction);
        if (CurrentAction == endedAction)
        {
            CurrentAction = CharacterAction.None;
            OnActionEnded?.Invoke(endedAction);
            print(CurrentAction);
        }
    }

    private bool CanStartAction(CharacterAction newAction)
    {
        // 🔹 Aquí decides tus reglas de exclusión:
        switch (CurrentAction)
        {
            case CharacterAction.Dashing:
            case CharacterAction.LightAttack:
            case CharacterAction.HeavyAttack:
                // Ejemplo: no permitir salto ni cura mientras dure dash/ataque
                return false;

            case CharacterAction.Blocking:
                // Ejemplo: sí puedes curar mientras bloqueas (si quisieras)
                return newAction != CharacterAction.Jumping &&
                       newAction != CharacterAction.Dashing;

            case CharacterAction.Healing:
                // Mientras curas, no permites nada
                return false;
        }

        return true; // Si no hay acción crítica, se permite
    }
}

public enum CharacterAction
{
    None,
    Idle,
    Moving,
    Jumping,
    Dashing,
    Blocking,
    LightAttack,
    HeavyAttack,
    Healing,
    UsingMap
}
