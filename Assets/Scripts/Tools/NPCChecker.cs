using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCChecker : MonoBehaviour
{
    private static NPCChecker _instance;

    public static NPCChecker Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("NPCChecker");
                _instance = singletonObject.AddComponent<NPCChecker>();
            }
            return _instance;
        }
        
    }

    PlayerInteract _playerInteract;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        //_playerInteract = GetComponent<PlayerInteract>();
    }

    //public List<int> GetInteractableNPC()
    //{
    //    Dictionary<string, int> NPCTable = new Dictionary<string, int>() 
    //    {
    //        {"The Witch of TRUTH", 0},
    //        {"Mayor", 1},
    //        {"Healer", 2},
    //        {"Woodcutter", 3},
    //        {"Peasant", 4},
    //        {"Boy", 5},
    //        {"Merchant", 6},
    //        {"passerby", 7},

    //        {"hunter of the witch", 11},
    //        {"The Witch of VENEER", 12},
    //        {"young witch", 13}
    //    };

    //    List<int> result = new List<int>();

    //    foreach(Collider2D collider2d in _playerInteract.NPCColliders)
    //    {
    //        foreach(KeyValuePair<string, int> pair in  NPCTable)
    //        {
    //            if(collider2d.gameObject.name == pair.Key)
    //            {
    //                result.Add(pair.Value);
    //            }
    //        }
    //    }

    //    return result;
    //}

    public int GetEnermyAliveNum()
    {
        GameObject[] enermies = GameObject.FindGameObjectsWithTag("Enermy");
        int result = enermies.Length;

        for (int i = 0; i < enermies.Length; i++)
        {
            if (enermies[i].transform.GetChild(1).GetComponent<NPCStatusEffect>().Dead)
                result--;
        }

        Debug.Log($"found enermy num {enermies.Length} alive {result}");

        return result;
    }
}
