using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCure : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable() {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += HandleCureInput;
        }
    }

    private void OnDisable() 
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered -= HandleCureInput;
        }
    }

    private void HandleCureInput(string actionName)
    {
        if (actionName == "Cure")
        { 
            CureController.Instance.TryUseCure();
        }
    }
}
