using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private ItemUI itemUI;
    private Image image;
    private Button button;

    Color originalColor = new Color(1, 1, 1, 1);
    Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1);
    Color pressColor = new Color(0.6f, 0.6f, 0.6f, 1);

    private void Awake()
    {
        itemUI = GetComponent<ItemUI>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemUI.Empty) return;

        if (itemUI.Item is ConsumableItem)
        {
            ConsumableItem consumableItem = itemUI.Item as ConsumableItem;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                consumableItem.Use(itemUI.Index);
            }

            return;
        }

        if (itemUI.Item is WeaponItem)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                PlayerHand.Instance.EquipL(itemUI.Item as WeaponItem);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                PlayerHand.Instance.EquipR(itemUI.Item as WeaponItem);
            }
        }

        if (itemUI.Item is ProjectileItem)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                PlayerHand.Instance.EquipProjectile(itemUI.Item as ProjectileItem);
            }
        }

        GameEvents.Instance.ItemsUpdated(itemUI.Index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = pressColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Right)
        {
            image.color = hoverColor;
        }
    }
}
