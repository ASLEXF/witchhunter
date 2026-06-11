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

    public virtual void Enter() 
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