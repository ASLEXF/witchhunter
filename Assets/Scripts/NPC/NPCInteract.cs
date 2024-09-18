using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class NPCInteract : MonoBehaviour
{
    NPCStatusEffect status;
    SpriteRenderer spriteRenderer;
    GameObject Items;

    public string raceName;
    [SerializeField] bool[] isInteractableArray = new bool[3];
    public bool isInteractable;
    [SerializeField] private Vector3 UIOffset;

    string interactConfigFilePath = "Assets/Config/InteractableNPC.json";
    string itemConfigFilePath = "Assets/Config/NPCItems.json";

    private void Awake()
    {
        status = transform.parent.GetChild(1).GetComponent<NPCStatusEffect>();
        spriteRenderer = transform.parent.GetChild(0).GetComponent<SpriteRenderer>();
        Items = transform.parent.Find("Items").gameObject;
    }

    private void Start()
    {
        loadInteractable(raceName);
    }

    private void Update()
    {
        
    }

    private void loadInteractable(string raceName)
    {
        if (File.Exists(interactConfigFilePath))
        {
            string json = File.ReadAllText(interactConfigFilePath);
            var configs = JsonConvert.DeserializeObject<Dictionary<string, List<NPCInteractConfigEntry>>>(json);
            if (configs == null)
            {
                Debug.LogError("Failed to deserialize JSON.");
                return;
            }
            if (configs.TryGetValue(raceName, out List<NPCInteractConfigEntry> config))
            {
                for (int i = 0; i < isInteractableArray.Length; i++)
                {
                    isInteractableArray[i] = config[i].isInteractable;
                }
            }
            else
            {
                Debug.LogError("No configurations found for the specified NPC interactable.");
            }
        }
        else
        {
            Debug.LogError($"Config file not found at {interactConfigFilePath}");
        }
    }

    public void UpdateIsInteractable()
    {
        isInteractable = isInteractableArray[(int)status.lifeStatus];
    }

    public void Interacted()
    {
        if (isInteractable)
        {
            switch (status.lifeStatus)
            {
                case NPCLifeStatusEnum.Alive:
                    Talk();
                    break;
                case NPCLifeStatusEnum.DeadItem:
                    GetBodyItem();
                    break;
                case NPCLifeStatusEnum.DeadEmpty:
                    PickUpBody();
                    break;
            }
        }
    }

    private void Talk()
    {
        Debug.Log("NPC Talk");
    }

    private void GetBodyItem()
    {
        Debug.Log("NPC GetNPCItem");
        
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a);
        GenerateItems(raceName);
    }

    public void GenerateItems(string raceName)
    {
        if (File.Exists(itemConfigFilePath))
        {
            string json = File.ReadAllText(itemConfigFilePath);
            var configs = JsonConvert.DeserializeObject<Dictionary<string, List<NPCItemConfigEntry>>>(json);
            if (configs == null)
            {
                Debug.LogError("Failed to deserialize JSON.");
                return;
            }
            if (configs.TryGetValue(raceName, out List<NPCItemConfigEntry> config))
            {
                foreach (var configItem in config)
                {
                    GenerateItem(configItem.itemID, Items, Random.Range(configItem.min, configItem.max + 1));
                }
            }
            else
            {
                Debug.LogError("No configurations found for the specified NPC items.");
            }
        }
        else
        {
            Debug.LogError($"Config file not found at {interactConfigFilePath}");
        }
    }

    void GenerateItem(int itemID, GameObject parentGameObj, int number)
    {
        GameObject gameObject = new GameObject();

    }

    private void PickUpBody()
    {
        Debug.Log("NPC PickUpBody");
    }
}
