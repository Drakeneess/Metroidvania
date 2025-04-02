using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;

public class Player : Character
{
    public float emotionalUseRate=1;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        UpdateOnPhysicalHealth();
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
        UpdateOnPhysicalHealth();
    }
    protected override void RestorePhysicalHealth(float amount)
    {
        base.RestorePhysicalHealth(amount);
        UpdateOnPhysicalHealth();
    }
    public override void UseMentalPulse(float amount)
    {
        amount*=emotionalUseRate;
        base.UseMentalPulse(amount);
    }

    private void UpdateOnPhysicalHealth(){
        if(CurrentPhysicalHealth>=physicalHealth*0.6){
            UpdateColorBar(Color.blue);
        }
        else if(CurrentPhysicalHealth>=physicalHealth*0.3){
            UpdateColorBar(Color.yellow);
        }
        else{
            UpdateColorBar(Color.red);
        }
    }
    private void UpdateColorBar(Color color) {
        DualShockGamepad dualShockGamepad = DualShockGamepad.current;
        if(dualShockGamepad!=null){
            dualShockGamepad.SetLightBarColor(color);
        }  
    }
}
