using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] UIInteract DropItemUI, NPCTalkUI, NPCCollectUI, NPCPickUpUI, InteractiveUI;

    Collider2D currentCollider;
    InteractTypeEnum type;
    [SerializeField] private List<Collider2D> DropItemColliders = new List<Collider2D>();
    [SerializeField] public List<Collider2D> NPCColliders = new List<Collider2D>();
    [SerializeField] private List<Collider2D> InteractiveColliders = new List<Collider2D>();

    bool isInteracted = false;  // don't interact twice


    private void Start()
    {
        GameEvents.Instance.OnInteractableUpdated += updateColliders;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void updateColliders()
    {
        if (isInteracted)
        {
            clearInteractedCollider();
            isInteracted = false;
        }

        if (DropItemColliders.Count > 0)
        {
            type = InteractTypeEnum.DropItem;
            currentCollider = DropItemColliders[0];
            ShowUI(DropItemUI);
            HideUI(NPCTalkUI);
            HideUI(NPCCollectUI);
            HideUI(NPCPickUpUI);
            HideUI(InteractiveUI);
        }
        else if (NPCColliders.Count > 0)
        {
            type = InteractTypeEnum.NPC;
            currentCollider = NPCColliders[0];

            if (currentCollider.GetComponentInChildren<NPCStatusEffect>().Alive)
            {
                ShowUI(NPCTalkUI);
                HideUI(NPCCollectUI);
                HideUI(NPCPickUpUI);
            }
            else if (currentCollider.GetComponentInChildren<NPCStatusEffect>().DeadItem)
            {
                HideUI(NPCTalkUI);
                ShowUI(NPCCollectUI);
                HideUI(NPCPickUpUI);
            }
            else if (currentCollider.GetComponentInChildren<NPCStatusEffect>().DeadEmpty)
            {
                HideUI(NPCTalkUI);
                HideUI(NPCCollectUI);
                ShowUI(NPCPickUpUI);
            }
            else
            {
                Debug.LogWarning($"Error when checking NPC {currentCollider.transform.parent.name} life status");
            }

            HideUI(DropItemUI);
            HideUI(InteractiveUI);
        }
        else if (InteractiveColliders.Count > 0)
        {
            type = InteractTypeEnum.Interactive;
            currentCollider = InteractiveColliders[0];
            ShowUI(InteractiveUI);
            HideUI(DropItemUI);
            HideUI(NPCTalkUI);
            HideUI(NPCCollectUI);
            HideUI(NPCPickUpUI);
        }
        else
        {
            HideUI(DropItemUI);
            HideUI(NPCTalkUI);
            HideUI(NPCCollectUI);
            HideUI(NPCPickUpUI);
            HideUI(InteractiveUI);
        }
    }

    void clearInteractedCollider()
    {
        if (currentCollider == null) return;

        switch (type)
        {
            case InteractTypeEnum.DropItem:
                {
                    DropItemColliders.Remove(currentCollider);
                    break;
                }
            case InteractTypeEnum.NPC:
                {
                    NPCColliders.Remove(currentCollider);
                    break;
                }
            case InteractTypeEnum.Interactive:
                {
                    InteractiveColliders.Remove(currentCollider);
                    break;
                }
        }
        currentCollider = null;
    }

    void ShowUI(UIInteract uIInteract)
    {
        if (uIInteract == null)
        {
            Debug.LogError("Interactive UI Panel is not assigned.");
            return;
        }

        Vector3 UIOffset = new Vector3();
        GameObject gameObject = currentCollider.transform.root.gameObject;
        switch (type)
        {
            case InteractTypeEnum.DropItem:
                {
                    DroppedItem script = gameObject.GetComponentInChildren<DroppedItem>();
                    if (script != null)
                    {
                        UIOffset = script.UIOffset;
                    }
                    break;
                }
            case InteractTypeEnum.NPC:
                {
                    NPCInteract script = gameObject.GetComponentInChildren<NPCInteract>();
                    if (script != null)
                    {
                        UIOffset = script.UIOffset;
                    }
                    break;
                }
            case InteractTypeEnum.Interactive:
                {
                    Interactive script = gameObject.GetComponentInChildren<Interactive>();
                    if (script != null)
                    {
                        UIOffset = script.UIOffset;
                    }

                    break;
                }
        }

        uIInteract.Show();
        uIInteract.transform.position = currentCollider.transform.position + UIOffset;
    }

    void HideUI(UIInteract uIInteract)
    {
        uIInteract.Hide();
    }

    public void Interact()
    {
        if (currentCollider != null)
        {
            Debug.Log($"interact gameObject {currentCollider.gameObject}");
            GameObject gameObject = currentCollider.transform.root.gameObject;
            switch (type)
            {
                case InteractTypeEnum.DropItem:
                {
                    DroppedItem script = gameObject.GetComponentInChildren<DroppedItem>();
                    if (script != null)
                    {
                        script.Interacted();
                    }

                    // game object get destroyed

                    break;
                }
                case InteractTypeEnum.NPC:
                {
                    NPCInteract script = gameObject.GetComponentInChildren<NPCInteract>();
                    if (script != null)
                    {
                        script.Interacted();
                    }

                    isInteracted = true;
                    GameEvents.Instance.InteractableUpdated();

                    break;
                }
                case InteractTypeEnum.Interactive:
                {
                    Interactive script = gameObject.GetComponentInChildren<Interactive>();
                    if (script != null)
                    {
                        script.Interacted();
                    }

                    isInteracted = true;
                    GameEvents.Instance.InteractableUpdated();

                    break;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DropItem"))
        {
            DropItemColliders.Add(collision);
        }
        else if (collision.CompareTag("NPC") && collision.transform.root.GetComponentInChildren<NPCInteract>().isInteractable)
        {
            NPCColliders.Add(collision);
        }
        else if (collision.CompareTag("Interactive"))
        {
            InteractiveColliders.Add(collision);
        }
        else return;

        GameEvents.Instance.InteractableUpdated();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DropItem"))
        {
            DropItemColliders.Remove(collision);
        }
        else if (collision.CompareTag("NPC") && collision.transform.root.GetComponentInChildren<NPCInteract>().isInteractable)
        {
            NPCColliders.Remove(collision);
        }
        else if (collision.CompareTag("Interactive"))
        {
            InteractiveColliders.Remove(collision);
        }
        else return;

        DialogBox.Instance.ClearAndHide();  // NOTE: remove dialogbox when leaving npc

        GameEvents.Instance.InteractableUpdated();
    }

    private void OnDestroy()
    {
        GameEvents.Instance.OnInteractableUpdated -= updateColliders;
    }
}
