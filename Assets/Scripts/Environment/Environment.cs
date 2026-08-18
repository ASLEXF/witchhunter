using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WitchHunter
{
    public class Environment : Singleton<Environment>
    {
        GameObject projectiles;

        public GameObject Projectiles
        {
            get { return projectiles; }
        }

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
}
