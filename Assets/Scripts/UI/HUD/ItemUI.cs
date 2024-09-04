using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemUI : MonoBehaviour
{
    Image image;
    GameObject item;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void UpdateSprite()
    {
        Item item = transform.GetChild(0).gameObject.GetComponent<Item>();
        if (item != null)
        {
            getSprite(item.icon, image);
        }
    }

    private void getSprite(string icon, Image image)
    {
        Addressables.LoadAssetAsync<Sprite>(icon).Completed += handle => 
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                image.sprite = handle.Result;
            }
            else
            {
                Debug.LogError("Addressables PlayableAsset not found: " + icon);
            }
        };
    }
}
