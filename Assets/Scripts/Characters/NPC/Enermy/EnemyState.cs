public abstract class EnemyState
{
    protected EnemyAIController enemy;

    public EnemyState(EnemyAIController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}