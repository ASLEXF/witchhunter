using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    Animator animator;

    Vector3 attackDirection;

    public float moveDuration = 0f;
    public float moveSpeed = 0f;
    //private float moveDistance = 0f;
    private float elapsedTime = 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
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

    #region Called by input

    public void AttackL()
    {
        if (PlayerHand.Instance.IsLEmpty) return;

        weaponAnimationStructure = PlayerHand.Instance.WeaponL.Attack();
        handleWeaponAnimationStructure();
    }

    public void ChargingAttackL()
    {
        if (PlayerHand.Instance.IsLEmpty) return;

        weaponAnimationStructure = PlayerHand.Instance.WeaponL.ChargingAttack();
        handleWeaponAnimationStructure();
    }

    public void ChargeAttackL(float chargingTime)
    {
        if (PlayerHand.Instance.IsLEmpty) return;

        weaponAnimationStructure = PlayerHand.Instance.WeaponL.ChargeAttack();
        handleWeaponAnimationStructure();
    }

    public void AttackR()
    {
        if (PlayerHand.Instance.IsREmpty) return;

        weaponAnimationStructure = PlayerHand.Instance.WeaponR.Attack();
        handleWeaponAnimationStructure();
    }

    public void ChargingAttackR()
    {
        if (PlayerHand.Instance.IsREmpty) return;

        weaponAnimationStructure = PlayerHand.Instance.WeaponR.ChargingAttack();
        handleWeaponAnimationStructure();
    }

    public void ChargeAttackR(float chargingTime)
    {
        if (PlayerHand.Instance.IsREmpty) return;

        weaponAnimationStructure = PlayerHand.Instance.WeaponR.ChargeAttack();
        handleWeaponAnimationStructure();
    }

    #endregion

    #region Triggered by animation

    private void MeetChargingTime()
    {
        // while meeting minimun charging time for charging movements
        animator.SetTrigger(Animator.StringToHash("MeetChargingTime"));
    }

    public void StopChargingL()
    {
        //animator.SetBool(Animator.StringToHash("SwordCharging"), false);
    }

    public void StopChargingR()
    {
        //animator.SetBool(Animator.StringToHash("BowCharging"), false);
    }

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
        PlayerController.Instance.CanMove = true;
    }

    private void stopMovement()
    {
        PlayerController.Instance.CanMove = false;
    }

    private void canAttack()
    {
        PlayerController.Instance.CanAttack = true;
    }

    private void stopAttacking()
    {
        PlayerController.Instance.CanAttack = false;
    }

    private int comboCount;

    public void AttackEndHandler()
    {
        if (weaponAnimationStructure.integerName == null) return;

        attackCounter++;
        if (attackResetter != null) StopCoroutine(attackResetter);
        attackResetter = StartCoroutine(resetAttackCounter());
        if (attackCounter >= comboCount)
            attackCounter = 0;

        animator.SetInteger(weaponAnimationStructure.integerName, attackCounter);
    }

    private void startArchery()
    {
        if (PlayerHand.Instance.IsProjectileEmpty) return;

        transform.parent.Find("InHand").GetComponentInChildren<IProjectile>().UpdatePosition(PlayerController.Instance.Direction, true);
    }

    private void updateArchery()
    {
        if (PlayerHand.Instance.IsProjectileEmpty) return;

        transform.parent.Find("InHand").GetComponentInChildren<IProjectile>().UpdatePosition(PlayerController.Instance.Direction);
    }

    private Vector2 arrowSpeed;

    private void setArrowSpeedLow()
    {
        arrowSpeed = new Vector2(8.0f, 0) * PlayerController.Instance.Direction.x;
        // TODO: set low damage
    }
    
    private void setArrowSpeedHigh()
    {
        arrowSpeed = new Vector2(16.0f, 0) * PlayerController.Instance.Direction.x;
        // TODO: set high damage
    }

    private void generateProjectile()
    {
        transform.parent.Find("InHand").GetComponentInChildren<IProjectile>().Shoot(arrowSpeed, transform.position.y - 2.0f);
    }

    #region Triggers

    public GameObject SwordAttack1;
    public GameObject SwordAttack2;
    public GameObject SwordChargeAttack;

    void EnableSwordAttack1Trigger() => SwordAttack1.SetActive(true);
    void EnableSwordAttack2Trigger() => SwordAttack2.SetActive(true);
    void EnableSwordChargeAttackTrigger() => SwordChargeAttack.SetActive(true);

    void DisableSwordAttack1Trigger() => SwordAttack1.SetActive(false);
    void DisableSwordAttack2Trigger() => SwordAttack2.SetActive(false);
    void DisableSwordChargeAttackTrigger() => SwordChargeAttack.SetActive(false);

    #endregion

    #endregion

    #region Counter

    private int attackCounter = 0;
    Coroutine attackResetter;

    public void ResetAttackCounter()
    {
        attackCounter = 0;
        animator.SetInteger(weaponAnimationStructure.integerName, attackCounter);
    }

    IEnumerator resetAttackCounter()
    {
        yield return new WaitForSeconds(2);

        ResetAttackCounter();
    }

    #endregion

    #region Trigger Animation

    private WeaponAnimationStructure weaponAnimationStructure;

    private void handleWeaponAnimationStructure()
    {
        if (!weaponAnimationStructure.hasAttack) return;

        if (weaponAnimationStructure.integerName != null &&
            weaponAnimationStructure.comboCount > 1)
        {
            comboCount = weaponAnimationStructure.comboCount;
        }

        if (weaponAnimationStructure.triggerName != null &&
            checkCondition(weaponAnimationStructure.attackCondition))
        {
            if (weaponAnimationStructure.attackCondition == AttackCondition.HasArrow)
            {
                startArchery();
            }
            animator.SetTrigger(weaponAnimationStructure.triggerName);
        }
            
    }

    private bool checkCondition(AttackCondition? attackCondition)
    {
        if (attackCondition == null) return true;
        switch (attackCondition)
        {
            case AttackCondition.HasArrow:
                return !PlayerHand.Instance.IsProjectileEmpty;
            default:
                return true;
        }
    }

    #endregion
}
