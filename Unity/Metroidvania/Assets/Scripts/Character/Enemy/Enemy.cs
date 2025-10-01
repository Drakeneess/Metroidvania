using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    public float damage = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
            player.TakePhysicalDamage(damage, this);
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    public override void TakePhysicalDamage(float dmg, Character damager)
    {
        base.TakePhysicalDamage(dmg, damager);
        
        FeedbackManager.Instance.TriggerHitStop(0.1f);
        CameraShaker.Instance.Shake(0.05f,0.1f);
    }
}
