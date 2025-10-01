using UnityEngine;

public class PlayerHealthBar : HealthBarController
{
    protected override void Start()
    {
        target = FindObjectOfType<Player>();
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
}
