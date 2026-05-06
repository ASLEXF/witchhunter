using UnityEngine;

public interface IItem
{
    Item GetItem();
    void Hide();
    void Show();
}

public interface IConsumableItem: IItem
{
    //void UseItem();
}

public interface IWeaponItem: IItem
{
    //WeaponAnimationStructure Attack();

    //WeaponAnimationStructure ChargingAttack();
}

public interface IProjectile: IItem
{
    void UpdatePosition(Vector2 direction, bool reset = false);

    void SetDamage(int damage);

    void Shoot(Vector2 force, float floorHeight);

    void Hit(Collider2D other);
}

public interface  ICharacterAnimation
{
    void AttackAnimation();
}