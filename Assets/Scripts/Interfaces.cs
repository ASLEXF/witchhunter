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
    void UpdatePosition();

    void Shoot(Vector2 force, float floorHeight);
}