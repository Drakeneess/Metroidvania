using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlocking : MonoBehaviour
{
    CharacterMovement characterMovement;
    // Start is called before the first frame update
    void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
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
            InputActionController.Instance.OnFloatInput += HandleBlockInput;
        }
    }

    private void HandleBlockInput(string actionName, float value)
    {
        if (actionName == "Block")
        {
            if (value != 0)
            {
                PlayerAnimationController.SetBlocking();
            }
            else
            {
                PlayerAnimationController.SetWalkState(characterMovement.HorizontalInput!=0, true);
            }
        }
    }
}
