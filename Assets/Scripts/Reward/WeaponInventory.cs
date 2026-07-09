using System;
using System.Collections.Generic;
using System.Linq;
using Analytics;
using SO;
using UnityEngine;

namespace Reward
{
    public class WeaponInventory : MonoBehaviour
    {
        private const string CurrentWeaponKey = "CurrentWeapon";
        private const string UnlockedWeaponsKey = "UnlockedWeapons";

        [SerializeField] private WeaponDatabase weaponDatabase;
        [SerializeField] private WeaponData defaultWeapon;
        
        public static WeaponInventory Instance { get; private set; }
        
        private HashSet<string> unlockedWeaponIds = new();
        
        public WeaponData CurrentWeapon { get; private set; }
        public int TotalCount => weaponDatabase.Weapons.Count;
        public int UnlockedCount => unlockedWeaponIds.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            //ResetUnlockedWeapons();
            Load();
            EnsureDefaultWeapon();
        }

        public void UnlockWeapon(WeaponData weapon)
        {
            if (weapon == null || string.IsNullOrEmpty(weapon.WeaponId))
                return;

            if (unlockedWeaponIds.Add(weapon.WeaponId))
            {
                Save();
                Debug.Log($"Weapon unlocked: {weapon.WeaponName}");
            }
            else
            {
                Debug.Log($"Weapon already unlocked: {weapon.WeaponName}");
            }
        }

        public bool IsUnlocked(WeaponData weapon)
        {
            return weapon != null &&
                   !string.IsNullOrEmpty(weapon.WeaponId) &&
                   unlockedWeaponIds.Contains(weapon.WeaponId);
        }

        private void Load()
        {
            string saved = PlayerPrefs.GetString(UnlockedWeaponsKey, "");

            unlockedWeaponIds = saved
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet();
            
            string currentWeaponId = PlayerPrefs.GetString(CurrentWeaponKey, "");

            CurrentWeapon = weaponDatabase.GetWeaponById(currentWeaponId);
        }

        private void Save()
        {
            PlayerPrefs.SetString(UnlockedWeaponsKey, string.Join(",", unlockedWeaponIds));
            PlayerPrefs.Save();
        }
        
        public void ResetUnlockedWeapons()
        {
            unlockedWeaponIds.Clear();

            PlayerPrefs.DeleteKey(CurrentWeaponKey);
            PlayerPrefs.DeleteKey(UnlockedWeaponsKey);
            PlayerPrefs.Save();

            Debug.Log("Weapon inventory reset.");
        }
        
        public void EquipWeapon(WeaponData weapon)
        {
            if (!IsUnlocked(weapon))
                return;

            CurrentWeapon = weapon;

            PlayerPrefs.SetString(CurrentWeaponKey, weapon.WeaponId);
            PlayerPrefs.Save();

            Debug.Log($"Weapon equipped: {weapon.WeaponName}");
            AnalyticsManager.Instance.SendWeaponEquipped(weapon.WeaponName);
        }

        public bool IsEquipped(WeaponData weapon)
        {
            return CurrentWeapon.WeaponId == weapon.WeaponId;
        }

        private void EnsureDefaultWeapon()
        {
            if (defaultWeapon == null)
                return;
            
            unlockedWeaponIds.Add(defaultWeapon.WeaponId);
            
            if (CurrentWeapon == null)
                CurrentWeapon = defaultWeapon;

            Save();
            PlayerPrefs.SetString(CurrentWeaponKey, CurrentWeapon.WeaponId);
            PlayerPrefs.Save();
        }
    }
}