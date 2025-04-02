using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void TakePhysicalDamage(float damage)
    {
        base.TakePhysicalDamage(damage);
    }
    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }
}
