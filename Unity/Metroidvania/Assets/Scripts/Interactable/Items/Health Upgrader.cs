using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrader : Item
{
    public HealthType healthType;
    public float quantity= 20;
    public override void PickUpItem()
    {
        base.PickUpItem();
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.UpgradeHealth(healthType, quantity);
            Destroy(gameObject);
        }
    }
}
