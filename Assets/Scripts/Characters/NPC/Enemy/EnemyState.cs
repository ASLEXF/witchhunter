using UnityEngine;

public abstract class EnemyState
{
    protected EnemyAIController enemy;

    protected float hesitateTimer;
    protected float hesitateDuration;
    public bool IsHesitating { get; private set; }

    public EnemyState(EnemyAIController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter(EnemyState prevState = default) 
    { }

    public virtual void Update() 
    {
        UpdateHesitation();
    }

    public virtual void Exit() 
    {
        StopHesitate();
    }

    protected void StartHesitate(float duration)
    {
        Debug.Log($"Enemy hesitates for {duration} seconds.");
        enemy.Agent.ResetPath();
        enemy.Agent.isStopped = true;
        enemy.Agent.velocity = Vector3.zero;
        enemy.Animator.SetBool("IsWalking", false);
        enemy.Animator.SetBool("IsRunning", false);
        
        hesitateDuration = duration;
        hesitateTimer = 0f;
        IsHesitating = duration > 0f;
    }

    public void StopHesitate()
    {
        hesitateTimer = 0f;
        hesitateDuration = 0f;
        IsHesitating = false;
    }

    private void UpdateHesitation()
    {
        if (!IsHesitating)
            return;

        hesitateTimer += Time.deltaTime;

        if (hesitateTimer >= hesitateDuration)
        {
            IsHesitating = false;
        }
    }
}