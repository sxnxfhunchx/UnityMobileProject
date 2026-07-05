using System;
using Reward;
using SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WeaponSlotView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button button;

        public WeaponData WeaponData { get; private set; }
        
        private Action<WeaponSlotView> onSelected;

        public void Initialize(WeaponData weaponData, Action<WeaponSlotView> callback)
        {
            WeaponData = weaponData;
            onSelected = callback;

            bool isUnlocked = WeaponInventory.Instance.IsUnlocked(WeaponData);
            bool isEquipped = WeaponInventory.Instance.IsEquipped(WeaponData);

            iconImage.sprite = WeaponData.Icon;
            nameText.text = WeaponData.WeaponName;

            if (!isUnlocked)
            {
                statusText.text = "LOCKED";
                button.interactable = false;
            }
            else if (isEquipped)
            {
                statusText.text = "EQUIPPED";
                button.interactable = false;
            }
            else
            {
                statusText.text = "";
                button.interactable = true;
            }

            iconImage.color = isUnlocked ? Color.white : Color.black;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            onSelected?.Invoke(this);
        }
    }
}