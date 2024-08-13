using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    Vector3 attackDirection;

    public GameObject col;

    public float moveDuration = 0f;
    public float moveSpeed = 0f;
    private float moveDistance = 0f;
    private float elapsedTime = 0f;

    private int attackCounter = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
        //rb = PlayerController.Instance.GetComponent<Rigidbody2D>();
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

            //float t = Mathf.Clamp01(elapsedTime / moveDuration);

            //rb.MovePosition(transform.position + attackDirection * moveSpeed * Time.deltaTime);

            //if (t >= 1f)
            //{
            //    moveDuration = 0f;
            //    moveSpeed = 0f;
            //    flag = true;
            //}
        }
    }

    public void AttackEndHandler()
    {
        if (attackCounter < 1)
        {
            attackCounter++;
        }
        else
        {
            attackCounter = 0;
        }

        animator.SetInteger("Counter", attackCounter);

        DisableAttackTrigger();
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

    private void canMove()
    {
        PlayerController.Instance.canMove = true;
    }

    #endregion

    #region Trigger

    void EnableAttackTrigger() => col.SetActive(true);

    void DisableAttackTrigger() => col.SetActive(false);

    #endregion

    #region Counter

    public void ResetAttackCounter() => attackCounter = 0;

    #endregion

    #region Weapon

    //private WeaponBase GetWeapon(int index)
    //{
    //    WeaponBase weapon = null;
    //    return weapon;
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enermy"))
        {
            //collision.GetComponent<Health>().TakeDamage(2);  // TODO
            Debug.Log("hit enermy");
        }
    }

    #endregion
}
