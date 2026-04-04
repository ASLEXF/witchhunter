using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : Singleton<Environment>
{
    GameObject projectiles;

    protected override void Awake()
    {
        base.Awake();

        projectiles = new GameObject("Projectiles");
        projectiles.transform.SetParent(gameObject.transform);
    }

    public void AddProjectile(GameObject projectile)
    {
        projectile.transform.SetParent(projectiles.transform);
    }
}
