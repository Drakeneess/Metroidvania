using UnityEngine;

public class EnemyHealthBar : HealthBarController
{
    [SerializeField] private Enemy enemy;

    protected override void Start()
    {
        if (enemy == null) enemy = GetComponentInParent<Enemy>();
        target = enemy;
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
}
