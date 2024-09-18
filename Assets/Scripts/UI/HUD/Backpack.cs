using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    private static Backpack instance;

    public static Backpack Instance
    {
        get { return instance; }
    }

    [SerializeField] List<GameObject> items;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        initializeItems();
        gameObject.SetActive(false);
    }

    private void initializeItems()
    {
        items = new List<GameObject>();
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = transform.GetChild(i).gameObject;
        }
    }

    private void updateItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Image image = items[i].GetComponent<Image>();
            GameObject itemObj = items[i].transform.GetChild(2).gameObject;
            if (itemObj != null)
            {
                Item item = itemObj.GetComponent<Item>();
                Text amount = items[i].transform.Find("Amount").GetComponent<Text>();
                if (item is ComsumableItem)
                {
                    ComsumableItem comsumableItem = item as ComsumableItem;
                    if (comsumableItem.amount == 0)
                    {
                        image.sprite = null;
                        amount.text = "";
                    }
                    else
                    {
                        SpriteRenderer itemSprite = itemObj.GetComponent<SpriteRenderer>();
                        image.sprite = itemSprite.sprite;
                        amount.text = comsumableItem.amount.ToString();
                    }
                }
                else if (item is MaterialItem)
                {
                    MaterialItem materialItem = item as MaterialItem;
                    if (materialItem.amount == 0)
                    {
                        image.sprite = null;
                        amount.text = "";
                    }
                    else
                    {
                        SpriteRenderer itemSprite = itemObj.GetComponent<SpriteRenderer>();
                        image.sprite = itemSprite.sprite;
                        amount.text = materialItem.amount.ToString();
                    }
                }
                else
                {
                    image.sprite = null;
                    amount.text = "";
                }
            }
            else
            {
                image.sprite = null;
            }
        }
    }

    public void Show()
    {
        updateItems();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
