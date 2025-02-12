using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName="";
    public float damage = 10f;
    public float range = 10f;
    public float maxTimeChargedAttack;
    public bool unlocked;
    public Sprite weaponImageUI;

    private void Start() {
        SetWeaponActive(false);
    }
    public void SetAsCurrentWeapon(){
        

    }

    public virtual void Attack(){
        SetWeaponActive(true);
    }
    
    public void SetWeaponActive(bool state){
        if(unlocked){
            gameObject.SetActive(state);
        }
        else{
            gameObject.SetActive(false);
        }
    }
    public virtual void LightAttack(int comboIndex){

    }
    public virtual void HeavyAttack(int comboIndex){

    }
}
