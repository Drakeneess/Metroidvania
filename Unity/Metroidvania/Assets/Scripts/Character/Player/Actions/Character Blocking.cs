using UnityEngine;

public class CharacterBlocking : MonoBehaviour
{
    private CharacterMovement characterMovement;
    private Player player;
    private PlayerAnimationController anim;

    private void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        player = GetComponent<Player>();
        anim = PlayerAnimationController.Instance;
    }

    private void OnEnable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnFloatInput += HandleBlockInput;
        }
    }

    private void OnDisable()
    {
        if (InputActionController.Instance != null)
        {
            InputActionController.Instance.OnFloatInput -= HandleBlockInput;
        }
    }

    private void HandleBlockInput(string actionName, float value)
    {
        return;
        if (actionName != "Block") return;

        if (value != 0)
        {
            anim.Block(true);
            PlayerActionLogger.Instance.Log("BlockStart");
        }
        else
        {
            anim.Block(false);

            // Actualizar estado base (Idle/Walk)
            bool isMoving = characterMovement.HorizontalInput != 0;
            anim.Move(isMoving);

            PlayerActionLogger.Instance.Log("BlockEnd");
        }
    }
}
