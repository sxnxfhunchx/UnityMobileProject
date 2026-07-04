using System;
using System.Collections.Generic;
using System.Linq;
using SO;
using UnityEngine;

namespace Reward
{
    public class WeaponInventory : MonoBehaviour
    {
        public static WeaponInventory Instance { get; private set; }

        private const string UnlockedWeaponsKey = "UnlockedWeapons";

        private HashSet<string> unlockedWeaponIds = new();

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
        }

        private void Save()
        {
            PlayerPrefs.SetString(UnlockedWeaponsKey, string.Join(",", unlockedWeaponIds));
            PlayerPrefs.Save();
        }
        
        public void ResetUnlockedWeapons()
        {
            unlockedWeaponIds.Clear();

            PlayerPrefs.DeleteKey(UnlockedWeaponsKey);
            PlayerPrefs.Save();

            Debug.Log("Weapon inventory reset.");
        }
    }
}