using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMeat : MonoBehaviour, IItem
{
    public ComsumableItem item;

    private void Awake()
    {
        item = new ComsumableItem(2, "raw meat", "heal 1 hp", "Assets/Addressables/Icons/raw_meat.png", 1);
        item.onUse = item =>
        {
            PlayerHealth.Instance.Heal(1);
            Debug.Log("Used raw meat, healed 1 hp");
        };
    }

    public void Use() => item.Use();

    public Item GetItem() => item;
}

//public class RawMeat : ComsumableItem
//{
//    public RawMeat() : base(2, "raw meat", "heal 1 hp", "Assets/Addressables/Icons/raw_meat.png", 1)
//    {

//    }

//    public override void Use()
//    {
//        PlayerHealth.Instance.Heal(1); 
//        Debug.Log("Used raw meat, healed 1 hp");
//    }
//}