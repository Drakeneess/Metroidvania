using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;

public class Player : Character
{
    public float emotionalUseRate = 1f;

    private Checkpoint lastCheckpoint;
    public Checkpoint LastCheckpoint
    {
        get => lastCheckpoint;
        set { lastCheckpoint = value; SetOnCheckpointPosition(); }
    }

    [SerializeField] private CharacterMovement movement;
    public CharacterMovement Movement => movement;

    protected override void Start()
    {
        base.Start();
        if (!movement)
            Debug.LogError("[Player] Falta CharacterMovement.");

        // Inicializar máximos desde SaveData
        var save = SaveDataController.Instance.saveData;
        health.SetMaxHealth(HealthType.Physical, save.physicalHealth);
        health.SetMaxHealth(HealthType.Mental, save.mentalHealth);
        health.SetMaxHealth(HealthType.Emotional, save.emotionalHealth);

        var pad = DualShockGamepad.current;
        if (pad != null) UpdateOnPhysicalHealth(pad);
        
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void TakePhysicalDamage(float damage, Character damager)
    {
        base.TakePhysicalDamage(damage, damager);

        string type = "Damaged";
        var extras = new List<string>
        {
            $"Amount Damage: {damage}"
        };

        if (damager != null)
            extras.Add($"Damager: {damager.name}");

        if (health.Get(HealthType.Physical) <= 0)
            type = "Died";

        PlayerActionLogger.Instance.Log(type, extras);

        var pad = DualShockGamepad.current;
        if (pad != null) 
            UpdateOnPhysicalHealth(pad);
    }

    public override void HealPhysical(float amount)
    {
        base.HealPhysical(amount);

        var pad = DualShockGamepad.current;
        if (pad != null) UpdateOnPhysicalHealth(pad);

        PlayerAnimationController.SetCurrentHealthPercentage(health.GetPercent(HealthType.Physical));
    }

    public void UseMentalPulse(float amount)
    {
        health.UsePulse(HealthType.Mental, amount * emotionalUseRate);
    }

    public override void Respawn()
    {
        SetOnCheckpointPosition();
        base.Respawn(); // health.Initialize()
    }

    public void SetOnCheckpointPosition()
    {
        if (lastCheckpoint != null)
            transform.position = lastCheckpoint.CheckpointPosition;
    }

    public void SetOnRefugee(Checkpoint newCheckpoint)
    {
        PlayerAnimationController.SetResting();
        LastCheckpoint = newCheckpoint;
    }

    public void RestOnRefugee()
    {
        health.RestoreFull();
    }

    public void UpgradeHealth(HealthType type, float quantity)
    {
        health.AddMax(type, quantity, alsoHealToMax: true);

        // Guardar en SaveData
        var save = SaveDataController.Instance.saveData;
        switch (type)
        {
            case HealthType.Physical:  save.physicalHealth  = health.GetMax(type); break;
            case HealthType.Mental:    save.mentalHealth    = health.GetMax(type); break;
            case HealthType.Emotional: save.emotionalHealth = health.GetMax(type); break;
        }
    }

    private void UpdateOnPhysicalHealth(DualShockGamepad pad)
    {
        float ratio = health.GetPercent(HealthType.Physical);
        if      (ratio >= 0.6f) pad.SetLightBarColor(Color.blue);
        else if (ratio >= 0.3f) pad.SetLightBarColor(Color.yellow);
        else                    pad.SetLightBarColor(Color.red);
    }

    protected override void Die()
    {
        
        // Aquí podrías abrir UI de muerte y llamar a Respawn después de un delay si quieres.
    }
}
