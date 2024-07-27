using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 2.0f;
    private Vector3 UIOffset = new Vector3(0f, 0.5f, 0f); // UI 元素相对角色的偏移量
    public UIInteract DropItemUI, NPCUI, InteractiveUI;

    private Collider2D currentCollider;
    private List<Collider2D> DropItemColliders = new List<Collider2D>();
    public List<Collider2D> NPCColliders = new List<Collider2D>();
    private List<Collider2D> InteractiveColliders = new List<Collider2D>();

    private void Update()
    {
        if(DropItemColliders.Count > 0)
        {
            currentCollider = DropItemColliders[0];
            ShowDropItemUI(currentCollider);
            HideNPCUI();
            HideInteractiveUI();
        }
        else if(NPCColliders.Count > 0)
        {
            currentCollider = NPCColliders[0];
            ShowNPCUI(currentCollider);
            HideDropItemUI();
            HideInteractiveUI();
        }
        else if(InteractiveColliders.Count > 0)
        {
            currentCollider = InteractiveColliders[0];
            ShowInteractiveUI(currentCollider);
            HideDropItemUI();
            HideNPCUI();
        }
        else
        {
            HideDropItemUI();
            HideNPCUI();
            HideInteractiveUI();
        }
    }

    private void ShowDropItemUI(UnityEngine.Collider2D collision)
    {
        DropItemUI.Show();
        Transform panelTransform = NPCUI.transform.Find("Collect");
        panelTransform.position = Camera.main.WorldToScreenPoint(collision.transform.position + UIOffset);
    }

    private void HideDropItemUI() => DropItemUI.Hide();

    private void ShowNPCUI(UnityEngine.Collider2D collision)
    {
        NPCUI.Show();
        Transform panelTransform = NPCUI.transform.Find("Talk");
        panelTransform.position = Camera.main.WorldToScreenPoint(collision.transform.position + UIOffset);
    }

    private void HideNPCUI() => NPCUI.Hide();

    private void ShowInteractiveUI(UnityEngine.Collider2D collision)
    {
        InteractiveUI.Show();
        Transform panelTransform = NPCUI.transform.Find("Intertact");
        panelTransform.position = Camera.main.WorldToScreenPoint(collision.transform.position + UIOffset);
    }

    private void HideInteractiveUI() => InteractiveUI.Hide();

    public void Interact()
    {
        if(currentCollider != null)
        {
            GameObject gameObject = currentCollider.transform.parent.gameObject;
            //Interaction script =  gameObject.GetComponent<Interaction>();
            //if(script != null)
            //{
            //    script.Interact();
            //}
        }
    }

    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
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
    }

    void OnTriggerExit2D(UnityEngine.Collider2D collision)
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
    }
}
