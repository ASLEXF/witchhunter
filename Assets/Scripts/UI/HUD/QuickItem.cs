using System.Collections;
using System.Collections.Generic;
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

        if (itemUI.Item is ComsumableItem)
        {
            ComsumableItem comsumableItem = itemUI.Item as ComsumableItem;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                comsumableItem.Use(comsumableItem);
                GameEvents.Instance.ItemsUpdated();  // TODO: optimize here
            }

            return;
        }

        if (itemUI.Item is WeaponItem)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (PlayerHand.Instance.WeaponL != null 
                    && itemUI.Item.id == PlayerHand.Instance.WeaponL.id)
                {
                    // pass
                }
                else if (PlayerHand.Instance.WeaponR != null 
                    && itemUI.Item.id == PlayerHand.Instance.WeaponR.id)
                {
                    PlayerHand.Instance.SwapLR();
                }
                else
                {
                    PlayerHand.Instance.EquipL(itemUI.Item as WeaponItem);
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (PlayerHand.Instance.WeaponR != null 
                    && itemUI.Item.id == PlayerHand.Instance.WeaponR.id)
                {
                    // pass
                }
                else if (PlayerHand.Instance.WeaponL != null 
                    && itemUI.Item.id == PlayerHand.Instance.WeaponL.id)
                {
                    PlayerHand.Instance.SwapLR();
                }
                else
                {
                    PlayerHand.Instance.EquipR(itemUI.Item as WeaponItem);
                }
            }
        }
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
