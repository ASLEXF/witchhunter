using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static UnityEditor.Progress;

[RequireComponent(typeof(Image))]
public class ItemUI : MonoBehaviour, IDropHandler
{
    [SerializeField] Item _item;
    Image image;
    TMP_Text _amount;
    DraggableItem draggableItem;

    private int _index;
    public Item Item
    {
        get { return _item; }
        set { _item = value; }
    }

    public TMP_Text Amount
    {
        get { return _amount; }
        set { _amount = value; }
    }

    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }
    public bool Empty
    { 
        get { return _item.id == 0; } 
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        _amount = transform.Find("Amount").GetComponent<TMP_Text>();
        draggableItem = GetComponent<DraggableItem>();
    }

    private void Start()
    {
        draggableItem.enabled = false;
    }

    public void UpdateUI()
    {
        if (_item != null && _item.id != 0)
        {
            updateSprite(_item.icon);
            if (_item is ComsumableItem)
            {
                ComsumableItem comsumableItem = _item as ComsumableItem;
                updateAmount(comsumableItem.amount);
            }
            else if (_item is MaterialItem)
            {
                MaterialItem materialItem = _item as MaterialItem;
                updateAmount(materialItem.amount);
            }
            else if (_item == PlayerHand.Instance.WeaponL)
            {
                _amount.text = "L";
            }
            else if (_item == PlayerHand.Instance.WeaponR)
            {
                _amount.text = "R";
            }
            else
            {
                _amount.text = "";
            }
            draggableItem.enabled = true;
        }
        else
        {
            updateSprite("Assets/Addressables/Icons/SquareWithBorder.png");
            _amount.text = "";
            draggableItem.enabled = false;
        }
        updateKey();
    }

    private void updateSprite(string icon)
    {
        if (icon != null && icon != "")
        {
            Addressables.LoadAssetAsync<Sprite>(icon).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    image.sprite = handle.Result;
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
                }
                else
                {
                    Debug.LogError("Addressables PlayableAsset not found: " + icon);
                }
            };
        }
        else
        {
            Debug.LogWarning($"can't find sprite: {icon}");
        }
    }

    private void updateAmount(int amount)
    {
        if (amount > 0)
        {
            _amount.text = amount.ToString();
        }
        else
        {
            updateSprite("Assets/Addressables/Icons/SquareWithBorder.png");
            image.sprite = null;
            _amount.text = "";
        }
    }

    private void updateKey()
    {
        // SettingsManager
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            ItemUI itemUI = eventData.pointerDrag.GetComponent<ItemUI>();
            if (_item == null)
            {
                _item = itemUI.Item;
                itemUI.Item = null;
            }
            else
            {
                Item tempItem = _item;
                _item = itemUI.Item;
                itemUI._item = tempItem;
            }
            draggableItem.OnEndDrag(eventData);
            GameEvents.Instance.ItemsUpdated();
        }
    }
}
