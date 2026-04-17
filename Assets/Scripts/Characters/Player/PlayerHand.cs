#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerHand : Singleton<PlayerHand>
{
    private void OnDestroy()
    {
        Addressables.Release(_prefabHandle);
    }

    private AsyncOperationHandle<GameObject> _prefabHandle;
    private GameObject projectileObj;

    private void addToHand(ProjectileItem item)
    {
        Addressables.LoadAssetAsync<GameObject>(item.prefab).Completed += handle =>
        {
            _prefabHandle = handle;
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject obj = Instantiate(handle.Result);
                obj.transform.SetParent(transform);
                obj.GetComponent<IItem>().Hide();
                obj.GetComponent<DroppedItem>().enabled = false;
            }
            else
            {
                Debug.LogError("Failed to load item prefab: " + item.prefab);
            }
        };
    }

    private void removeFromHand(ProjectileItem item)
    {
        if (projectileObj != null)
        {
            Destroy(projectileObj);
        }
    }

    #region Left Hand

    [SerializeField] private WeaponItem? weaponL;

    public WeaponItem? WeaponL
    {
        get { return weaponL; }
    }

    public bool IsLEmpty
    {
        get { return weaponL == null; }
    }

    public void EquipL(WeaponItem item)
    {
        if (item == weaponL)
        {
            UnequipL();
        }
        else if (item == weaponR)
        {
            SwapLR();
        }
        else
        {
            weaponL = item;

            // auto-equip projectile if the weapon is a bow
            if (item.id == 11)
            {
                tryEquipProjectile();
            }
        }
    }

    public void UnequipL()
    {
        weaponL = null;
    }

    #endregion

    #region Right Hand

    [SerializeField] private WeaponItem? weaponR;

    public WeaponItem? WeaponR
    {
        get { return weaponR; }
    }

    public bool IsREmpty
    {
        get { return weaponR == null; }
    }

    

    public void EquipR(WeaponItem item)
    {
        if (item == weaponR)
        {
            UnequipR();
        }
        else if (item == weaponL)
        {
            SwapLR();
        }
        else
        {
            weaponR = item;
        }
    }

    public void UnequipR()
    {
        weaponR = null;
    }

    public void SwapLR()
    {
        WeaponItem? temp = weaponL;
        weaponL = weaponR;
        weaponR = temp;
    }

    #endregion

    #region Projectile

    [SerializeField] private ProjectileItem? projectile;

    public ProjectileItem? Projectile
    {
        get { return projectile; }
        set { projectile = value; }
    }

    public bool IsProjectileEmpty
    {
        get { return projectile == null; }
    }

    public void EquipProjectile(ProjectileItem item)
    {
        if (item == projectile) return;

        if (projectile != null)
            UnequipProjectile();
        projectile = item;
        addToHand(item);
    }

    public void UnequipProjectile()
    {
        removeFromHand(projectile);
        projectile = null;
    }

    private void tryEquipProjectile()
    {
        if (projectile == null)
        {
            ProjectileItem? newProjectile = PlayerInventory.Instance.FindItem(13, true) as ProjectileItem;
            if (newProjectile != null)
            {
                EquipProjectile(newProjectile);
                return;
            }

            newProjectile = PlayerInventory.Instance.FindItem(14, true) as ProjectileItem;
            if (newProjectile != null)
            {
                EquipProjectile(newProjectile);
                return;
            }

            newProjectile = PlayerInventory.Instance.FindItem(15, true) as ProjectileItem;
            if (newProjectile != null)
            {
                EquipProjectile(newProjectile);
                return;
            }
        }
    }

    #endregion
}
