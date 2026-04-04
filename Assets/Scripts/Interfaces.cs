using UnityEngine;

public interface IItem
{
    Item GetItem();
}

public interface IConsumableItem
{
    void UseItem();
}

public interface IProjectile
{
    void UpdatePosition(Vector2 direction, bool reset = false);

    void Shoot(Vector2 force, float floorHeight);
}