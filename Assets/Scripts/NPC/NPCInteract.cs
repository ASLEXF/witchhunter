using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NPCInteract : MonoBehaviour
{
    NPCStatusEffect status;
    SpriteRenderer spriteRenderer;
    GameObject Items;

    public string raceName;

    [SerializeField] bool[] isInteractableStatus = new bool[3];
    public bool isInteractable;
    [SerializeField] public Vector3 UIOffset = new Vector3(0f, 0.5f, 0f);

    [SerializeField] string interactConfigFilePath = "Assets/Config/InteractableNPC.json";
    [SerializeField] string itemConfigFilePath = "Assets/Config/NPCItems.json";
    [SerializeField] string talkFilePath = "Assets/Config/NPCTalkFile.json"; 

    private void Awake()
    {
        status = transform.parent.Find("Status").GetComponent<NPCStatusEffect>();
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
                Debug.LogError($"Failed to deserialize JSON {interactConfigFilePath}.");
                return;
            }
            if (configs.TryGetValue(raceName, out List<NPCInteractConfigEntry> config))
            {
                for (int i = 0; i < isInteractableStatus.Length; i++)
                {
                    isInteractableStatus[i] = config[i].isInteractable;
                }

                UpdateIsInteractable();
            }
            else
            {
                Debug.LogError($"No configurations found for the NPC {raceName}.");
            }
        }
        else
        {
            Debug.LogError($"Config file not found at {interactConfigFilePath}.");
        }
    }

    public void UpdateIsInteractable()
    {
        isInteractable = isInteractableStatus[(int)status.lifeStatus];
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

        if (File.Exists(talkFilePath))
        {
            string json = File.ReadAllText(talkFilePath);
            var configs = JsonConvert.DeserializeObject<Dictionary<string, List<NPCTalkConfigEntry>>>(json);
            if (configs == null)
            {
                Debug.LogError($"Failed to deserialize JSON {talkFilePath}.");
                return;
            }
            if (configs.TryGetValue(raceName, out List<NPCTalkConfigEntry> config))
            {
                for (int i = 0; i < config.Count; i++)
                {
                    if (GameManager.Instance.stage <= config[i].stage)
                    {
                        DialogBox.Instance.LoadAndStartText(config[i].fileName);

                        return;
                    }
                }
            }
            else
            {
                Debug.LogError($"No configurations found for the NPC {raceName}.");
            }
        }
        else
        {
            Debug.LogError($"Config file not found at {interactConfigFilePath}");
        }
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
                Debug.LogError($"No configurations found for the NPC {raceName}.");
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
