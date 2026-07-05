using System.Collections.Generic;
using System.Linq;
using Reward;
using SO;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WeaponListController : MonoBehaviour
    {
        [SerializeField] private WeaponDatabase weaponDatabase;
        [SerializeField] private WeaponSlotView weaponViewPrefab;
        [SerializeField] private TMP_Text statusText;
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

            int total = WeaponInventory.Instance.TotalCount;
            int unlockedCount = WeaponInventory.Instance.UnlockedCount;
            
            statusText.text = $"{unlockedCount}/{total} unlocked";
            
            var sortedWeapons = weaponDatabase.Weapons
                .Where(w => w != null)
                .OrderByDescending(w => WeaponInventory.Instance.IsEquipped(w))
                .ThenByDescending(w => WeaponInventory.Instance.IsUnlocked(w))
                .ThenBy(w => w.WeaponName);
            
            foreach (WeaponData weaponData in sortedWeapons)
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