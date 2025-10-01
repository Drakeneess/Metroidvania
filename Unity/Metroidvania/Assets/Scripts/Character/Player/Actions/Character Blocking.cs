using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlocking : MonoBehaviour
{
    private CharacterMovement characterMovement;
    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnFloatInput += HandleBlockInput;
        }
    }

    private void OnDisable() {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnFloatInput -= HandleBlockInput;
        }
    }

    private void HandleBlockInput(string actionName, float value)
    {
        if (actionName == "Block")
        {
            if (value != 0)
            {
                PlayerAnimationController.SetBlocking();
                PlayerActionLogger.Instance.Log("BlockStart");
            }
            else
            {
                PlayerAnimationController.SetWalkState(characterMovement.HorizontalInput != 0, true);
                PlayerActionLogger.Instance.Log("BlockEnd");
            }
        }
    }
}
