using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private readonly List<Enemy> enemies = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary> Registrar enemigos al iniciar. </summary>
    public void Register(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    /// <summary> Por si alguno se destruye o desactiva permanentemente. </summary>
    public void Unregister(Enemy enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    /// <summary> Reinicia TODOS los enemigos a su estado inicial (posición, vida, estado, anim, IA). </summary>
    public void ResetAllEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.Respawn();
        }
    }
}
