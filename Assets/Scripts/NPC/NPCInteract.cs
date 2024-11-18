# nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;


public class NPCInteract : MonoBehaviour
{
    NPCStatusEffect status;
    SpriteRenderer spriteRenderer;
    NPCTasks? tasks;

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

        Transform? tasksTransform = transform.parent.Find("Tasks");
        if (tasksTransform is null)
            tasks = null;
        else
            tasks = tasksTransform.GetComponent<NPCTasks>();
    }

    private void Start()
    {
        loadInteractable(raceName);
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
        int id = 0;

        if (tasks != null && tasks.hasTask)
        {
            ITask? task;
            if ((task = tasks.GetTaskByStatus(TaskStatus.published)) != null)
            {
                // TODO: for now, only 1 task exist for each NPC at the same time
                if (task.CheckFinish())
                    id = task.GetTalkFileId(TaskStatus.finish);
                else
                    id = task.GetTalkFileId(TaskStatus.published);
            }
            else if (tasks.ActivateTasksByStage())
            {}
            else if((task = tasks.GetTaskByStatus(TaskStatus.finished)) != null)
            {
                id = task.GetTalkFileId(TaskStatus.finished);
            }
            else
            {
                Debug.LogWarning($"Unrecognizable task '{task}'!");
            }
        }

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
                    if (GameManager.Instance.Stage <= config[i].stage && id == config[i].id)
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

    private async void GetBodyItem()
    {
        Debug.Log("NPC GetNPCItem");

        Item[]? items = await GenerateItems(raceName);

        if (items != null)
        {
            foreach (Item item in items)
            {
                PlayerInventory.Instance.AddItem(item);
            }
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r / 2, spriteRenderer.color.g / 2, spriteRenderer.color.b / 2, spriteRenderer.color.a);

        status.lifeStatus = NPCLifeStatusEnum.DeadEmpty;
    }

    public async Task<Item[]?> GenerateItems(string raceName)
    {
        if (File.Exists(itemConfigFilePath))
        {
            string json = File.ReadAllText(itemConfigFilePath);
            var configs = JsonConvert.DeserializeObject<Dictionary<string, List<NPCItemConfigEntry>>>(json);
            if (configs == null)
            {
                Debug.LogError("Failed to deserialize JSON.");
                return null;
            }
            if (configs.TryGetValue(raceName, out List<NPCItemConfigEntry> config))
            {
                List<Task<Item>> tasks = new List<Task<Item>>();
                foreach (var configItem in config)
                {
                    tasks.Add(ItemManager.Instance.GenerateItem(configItem.itemID, Random.Range(configItem.min, configItem.max + 1)));
                }

                return await Task.WhenAll(tasks);
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

        return null;
    }

    private void PickUpBody()
    {
        Debug.Log("NPC PickUpBody");
    }
}
