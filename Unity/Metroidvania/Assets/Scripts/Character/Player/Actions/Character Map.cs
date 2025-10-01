using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMap : MonoBehaviour
{
    [SerializeField] private MapUIController mapUIController;
    void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered += HandleMapInput;
        }
    }

    void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnActionTriggered -= HandleMapInput;
        }
    }

    private void HandleMapInput(string actionName)
    {
        if (actionName != "Map") return;

        mapUIController.gameObject.SetActive(true);
        PlayerActionLogger.Instance.Log("Open Map");
    }
}
