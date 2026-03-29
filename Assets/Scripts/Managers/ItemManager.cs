using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AddressableAssets;
using System.Net;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditorInternal.Profiling.Memory.Experimental;
using System.Threading.Tasks;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] List<GameObject> objs = new List<GameObject>();
    public List<Item> Items = new List<Item>();
    [SerializeField] string itemConfigFilePath = "Assets/Config/Items.json";

    public async Task<Item> GenerateItem(int id, int amount = 1)
    {
        if (File.Exists(itemConfigFilePath))
        {
            string json = File.ReadAllText(itemConfigFilePath);
            List<ItemConfigEntry> items = JsonConvert.DeserializeObject<List<ItemConfigEntry>>(json);
            if (items == null)
            {
                Debug.LogError($"Failed to deserialize JSON {itemConfigFilePath}.");
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == id)
                {
                    var handle = Addressables.InstantiateAsync(items[i].prefab, Vector3.zero, Quaternion.identity);
                    await handle.Task;

                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Item item = handle.Result.GetComponent<IItem>().GetItem();

                        Destroy(handle.Result);

                        if (amount != 1)
                        {
                            if (item is ConsumableItem)
                            {
                                ConsumableItem comsumableItem = (ConsumableItem)item;
                                comsumableItem.amount = amount;
                            }
                            else if (item is MaterialItem)
                            {
                                MaterialItem materialItem = (MaterialItem)item;
                                materialItem.amount = amount;
                            }
                        }
                        return item;
                    }
                    else
                    {
                        Debug.LogError($"Failed to load prefab at address '{items[i].prefab}'.");
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"Config file not found at {itemConfigFilePath}");
        }

        return null;
    }

    //public List<Item> GetItem()
    //{
    //    List<Item> result = new List<Item>();
    //    foreach (var item in Items)
    //    {
    //        result.Add(item.DeepCopy());
    //    }
    //    Items.Clear();
    //    foreach (var obj in objs)
    //    {
    //        Destroy(obj);
    //    }
    //    objs.Clear();

    //    return result;
    //}
}
