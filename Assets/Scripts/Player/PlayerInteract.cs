using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Vector3 UIOffset = new Vector3(0f, 0.5f, 0f); // UI 元素相对角色的偏移量
    public UIInteract DropItemUI, NPCUI, InteractiveUI;

    Collider2D currentCollider;
    [SerializeField] private List<Collider2D> DropItemColliders = new List<Collider2D>();
    [SerializeField] public List<Collider2D> NPCColliders = new List<Collider2D>();
    [SerializeField] private List<Collider2D> InteractiveColliders = new List<Collider2D>();

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
        if (DropItemColliders.Count > 0)
        {
            currentCollider = DropItemColliders[0];
            ShowUI(DropItemUI);
            HideUI(NPCUI);
            HideUI(InteractiveUI);
        }
        else if (NPCColliders.Count > 0)
        {
            currentCollider = NPCColliders[0];
            ShowUI(NPCUI);
            HideUI(DropItemUI);
            HideUI(InteractiveUI);
        }
        else if (InteractiveColliders.Count > 0)
        {
            currentCollider = InteractiveColliders[0];
            ShowUI(InteractiveUI);
            HideUI(DropItemUI);
            HideUI(NPCUI);
        }
        else
        {
            HideUI(DropItemUI);
            HideUI(DropItemUI);
            HideUI(NPCUI);
        }
    }

    void ShowUI(UIInteract uIInteract)
    {
        if (uIInteract == null)
        {
            Debug.LogError("Interactive UI Panel is not assigned.");
            return;
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
            Debug.Log($"gameObject {currentCollider.gameObject}");
            GameObject gameObject = currentCollider.gameObject;
            NPCInteract script = gameObject.GetComponent<NPCInteract>();
            if (script != null)
            {
                script.Interacted();
            }
            GameEvents.Instance.InteractableUpdated();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DropItem"))
        {
            DropItemColliders.Add(collision);
        }
        else if (collision.CompareTag("NPC"))
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
        if(collision.CompareTag("DropItem"))
        {
            DropItemColliders.Remove(collision);
        }
        else if (collision.CompareTag("NPC"))
        {
            NPCColliders.Remove(collision);
        }
        else if (collision.CompareTag("Interactive"))
        {
            InteractiveColliders.Remove(collision);
        }
        else return;

        GameEvents.Instance.InteractableUpdated();
    }

    private void OnDestroy()
    {
        GameEvents.Instance.OnInteractableUpdated -= updateColliders;
    }
}
