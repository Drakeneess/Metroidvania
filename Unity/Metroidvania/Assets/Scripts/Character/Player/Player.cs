using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;

public class Player : Character
{
    public float emotionalUseRate = 1;
    private Checkpoint lastCheckpoint;
    public Checkpoint LastCheckpoint
    {
        set
        {
            lastCheckpoint = value;
            SetOnCheckpointPosition();
        }
        get { return lastCheckpoint; }
    }

    [SerializeField]
    private CharacterMovement characterMovement;
    public CharacterMovement CharacterMovement { get { return characterMovement; } }
    void Awake()
    {
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (characterMovement == null)
        {
            Debug.LogError("Player has no CharacterMovement script");
        }
        DualShockGamepad dualShockGamepad = DualShockGamepad.current;
        if (dualShockGamepad != null)
        {
            UpdateOnPhysicalHealth(dualShockGamepad);
        }
        vibrationTime = 0.06f;
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void Die()
    {
        base.Die();
    }
    public override void TakePhysicalDamage(float damage)
    {
        base.TakePhysicalDamage(damage);
        DualShockGamepad dualShockGamepad = DualShockGamepad.current;
        if (dualShockGamepad != null)
        {
            UpdateOnPhysicalHealth(dualShockGamepad);
        }
        PlayerAnimationController.SetCurrentHealthPercentage(GetPercentageHealth(HealthType.Physical));
    }
    public override void RestorePhysicalHealth(float amount)
    {
        base.RestorePhysicalHealth(amount);
        DualShockGamepad dualShockGamepad = DualShockGamepad.current;
        if (dualShockGamepad != null)
        {
            UpdateOnPhysicalHealth(dualShockGamepad);
        }
        PlayerAnimationController.SetCurrentHealthPercentage(GetPercentageHealth(HealthType.Physical));
    }
    public override void UseMentalPulse(float amount)
    {
        amount *= emotionalUseRate;
        base.UseMentalPulse(amount);
    }
    public override void Respawn()
    {
        SetOnCheckpointPosition();
        base.Respawn();
    }

    private void UpdateOnPhysicalHealth(DualShockGamepad dualShockGamepad)
    {
        if (CurrentPhysicalHealth >= physicalHealth * 0.6)
        {
            UpdateColorBar(Color.blue, dualShockGamepad);
        }
        else if (CurrentPhysicalHealth >= physicalHealth * 0.3)
        {
            UpdateColorBar(Color.yellow, dualShockGamepad);
        }
        else
        {
            UpdateColorBar(Color.red, dualShockGamepad);
        }
    }
    private void UpdateColorBar(Color color, DualShockGamepad dualShockGamepad)
    {
        if (dualShockGamepad != null)
        {
            dualShockGamepad.SetLightBarColor(color);
        }
    }

    public void SetOnCheckpointPosition()
    {
        if (lastCheckpoint != null)
        {
            transform.position = lastCheckpoint.CheckpointPosition;
        }
    }
    public void RestOnRefugee(Checkpoint newCheckpoint)
    {
        PlayerAnimationController.SetResting();
        RestorePhysicalHealth(physicalHealth);
        RestoreMentalHealth(mentalHealth);
        RestoreEmotionalHealth(emotionalHealth);
        LastCheckpoint = newCheckpoint;
    }
}
