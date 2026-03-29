using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour, IItem
{
    public ConsumableItem item;

    private void Awake()
    {
        item = new ConsumableItem(9, "Potion", "add 1 max hp", "Assets/Addressables/Icons/potion.png", 1);
        item.onUse = item => { PlayerHealth.Instance.AddMaxHealth(1); };
    }

    public void Use() => item.Use();

    public Item GetItem() => item;
}

//public class Potion : ComsumableItem
//{
//    public Potion() : base(9, "Potion", "add 1 max hp", "Assets/Addressables/Icons/potion.png", 1)
//    {
//    }

//    public override void Use()
//    {
//        PlayerHealth.Instance.AddMaxHealth(1);
//    }
//}
