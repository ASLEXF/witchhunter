using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AssetPreloader
{
    public static async Task PreloadAllIcons()
    {
        string[] icons =
        {
            "Icons/SquareWithBorder.png",
        };

        foreach (string icon in icons)
        {
            await Addressables.LoadAssetAsync<Sprite>(icon).Task;
        }
    }
}
