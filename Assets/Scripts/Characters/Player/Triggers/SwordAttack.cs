using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    PolygonCollider2D PlayerCollider;
    [SerializeField] BladeStats bladeStats;

    private void Awake()
    {
        PlayerCollider = transform.parent.parent.GetComponent<PolygonCollider2D>();
    }

    private void OnDisable()
    {
        hitColliders.Clear();
    }

    #region Hitbox trigger

    List<Collider2D> hitColliders = new List<Collider2D>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Sword hit: " + collision.name);
        if (
            !collision.isTrigger 
            || hitColliders.Contains(collision)
            || !collision.CompareTag("Enemy")
        )
            return;

        if (collision.name == "Animator")
        {
            collision.GetComponent<NPCAttacked>().GetAttacked(bladeStats.normalAttackDamage, bladeStats.normalAttackForce, PlayerCollider);
            hitColliders.Add(collision);
        }
    }

    #endregion
}
