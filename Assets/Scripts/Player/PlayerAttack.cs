using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private Vector3 attackDirection;

    public GameObject col;

    public WeaponBase currentWeapon;

    public float moveDuration = 0f;
    public float moveSpeed = 0f;
    private float moveDistance = 0f;
    private float elapsedTime = 0f;

    private int attackCounter = 1;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        
        currentWeapon = GetComponent<WeaponBase>();
    }

    private void Update()
    {
        bool flag = true;
        Vector3 initialPosition = new Vector3(0, 0, 0);
        Vector3 targetPosition = new Vector3(0, 0, 0);
        if (moveDuration > 0.1f)
        {
            if (flag)
            {
                elapsedTime = 0f;
                attackDirection = PositionLine.Instance.GetAttackDirection();
                flag = false;
            }

            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            rb.MovePosition(transform.position + attackDirection * moveSpeed * Time.deltaTime);

            if (t >= 1f)
            {
                moveDuration = 0f;
                moveSpeed = 0f;
                flag = true;
            }
        }
    }

    public void AttackEndHandler()
    {
        if (attackCounter < 2)
        {
            attackCounter++;
        }
        else
        {
            attackCounter = 1;
        }

        animator.SetInteger("Counter", attackCounter);

        DisableAttackCollider();
    }

    #region Movement
    public void MoveOnAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Sword_Attack_1"))
        {
            moveDuration = 0f;
            moveSpeed = 0f;
        }
        else if (stateInfo.IsName("Sword_Attack_2"))
        {
            moveDuration = 0.05f;
            moveSpeed = 15f;
        }
    }
    #endregion

    #region Collider

    void EnableAttackCollider() => col.SetActive(true);

    void DisableAttackCollider() => col.SetActive(false);

    #endregion

    #region Counter

    public void ResetAttackCounter() => attackCounter = 0;

    #endregion

    #region Weapon

    private WeaponBase GetWeapon(int index)
    {
        WeaponBase weapon = null;
        return weapon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enermy"))
        {
            collision.GetComponent<Health>().TakeDamage(2);  // TODO
        }
    }

    #endregion
}
