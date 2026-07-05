using System.Collections.Generic;
using Reward;
using SO;
using UnityEngine;

namespace UI
{
    public class WeaponListController : MonoBehaviour
    {
        [SerializeField] private WeaponDatabase weaponDatabase;
        [SerializeField] private WeaponSlotView weaponViewPrefab;
        [SerializeField] private Transform content;

        private readonly List<WeaponSlotView> slotViews = new();
        private WeaponSlotView selectedSlot;

        private void OnEnable()
        {
            RefreshList();
        }
        
        private void RefreshList()
        {
            ClearList();

            if (weaponDatabase == null || weaponDatabase.Weapons == null)
                return;
            
            foreach (WeaponData weaponData in weaponDatabase.Weapons)
            {
                if (weaponData == null)
                    continue;
                
                WeaponSlotView slot = Instantiate(weaponViewPrefab, content);
                slot.Initialize(weaponData, SelectSlot);
                slotViews.Add(slot);
            }
            
        }

        private void ClearList()
        {
            foreach (var slot in slotViews)
            {
                if (slot != null)
                    Destroy(slot.gameObject);
            }

            slotViews.Clear();
            selectedSlot = null;
        }

        private void SelectSlot(WeaponSlotView slot)
        {
            if (slot == null)
                return;

            WeaponInventory.Instance.EquipWeapon(slot.WeaponData);
            RefreshList();
        }
    }
}