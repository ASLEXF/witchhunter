using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;

    Vector3 attackDirection;
    Coroutine attackResetter;

    public GameObject SwordAttack1;
    public GameObject SwordAttack2;
    public GameObject SwordChargeAttack;

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
                attackDirection = PositionLine.Instance.GetClickDirection();
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

    private void MeetChargingTime()
    {
        // while meeting minimun charging time for charging movements
        animator.SetTrigger(Animator.StringToHash("MeetChargingTime"));
    }

    public void AttackL()
    {
        animator.SetTrigger(Animator.StringToHash("SwordAttack"));
    }

    public void ChargingAttackL()
    {
        animator.SetTrigger(Animator.StringToHash("SwordCharging"));
    }

    public void ChargeAttackL(float chargingTime)
    {
        animator.SetTrigger(Animator.StringToHash("SwordChargeAttack"));
    }

    public void StopChargingL()
    {
        //animator.SetBool(Animator.StringToHash("SwordCharging"), false);
    }

    public void AttackR()
    {
        //animator.SetTrigger(Animator.StringToHash("Support"));
    }

    public void ChargingAttackR()
    {
        animator.SetTrigger(Animator.StringToHash("BowCharging"));
    }

    public void ChargeAttackR(float chargingTime)
    {
        animator.SetTrigger(Animator.StringToHash("BowShoot"));
    }

    public void StopChargingR()
    {
        //animator.SetBool(Animator.StringToHash("BowCharging"), false);
    }

    public void AttackEndHandler()
    {
        if (attackCounter < 1)
        {
            attackCounter++;
            if (attackResetter != null) StopCoroutine(attackResetter);
            attackResetter = StartCoroutine(resetAttackCounter());
        }
        else
        {
            attackCounter = 0;
            if (attackResetter != null) StopCoroutine(attackResetter);
        }

        animator.SetInteger("SwordCounter", attackCounter);

        //DisableAttackTrigger();
    }

    IEnumerator resetAttackCounter()
    {
        yield return new WaitForSeconds(1.5f);

        attackCounter = 0;
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

    private void stopMovement()
    {
        PlayerController.Instance.canMove = false;
    }

    #endregion

    #region Trigger

    void EnableSwordAttack1Trigger() => SwordAttack1.SetActive(true);
    void EnableSwordAttack2Trigger() => SwordAttack2.SetActive(true);
    void EnableSwordChargeAttackTrigger() => SwordChargeAttack.SetActive(true);

    void DisableSwordAttack1Trigger() => SwordAttack1.SetActive(false);
    void DisableSwordAttack2Trigger() => SwordAttack2.SetActive(false);
    void DisableSwordChargeAttackTrigger() => SwordChargeAttack.SetActive(false);


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

    #endregion
}
