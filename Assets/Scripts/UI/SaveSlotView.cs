using System;
using Save;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotView : MonoBehaviour
{

    [SerializeField] private TMP_Text saveNameText;
    [SerializeField] private Image screenshot;
    [SerializeField] private Image background;
    [SerializeField] private Button button;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    
    private SaveFileData data;
    private Action<SaveSlotView> onSelected;

    public SaveFileData Data => data;
    
    public void Initialize(SaveFileData saveData, Action<SaveSlotView> selectedCallback)
    {
        data = saveData;
        onSelected = selectedCallback;

        string formattedDate = saveData.Date.ToString("dd.MM.yyyy HH:mm");
        saveNameText.text = data.SaveName + " \n" + formattedDate;
        SetThumbnail(data.Thumbnail);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnRowClicked);
    }

    public void SetSelected(bool selected)
    {
        background.color = selected ? selectedColor : normalColor;
    }
    
    public void SetThumbnail(Sprite sprite)
    {
        screenshot.sprite = sprite;
        screenshot.enabled = sprite != null;
        
        if (sprite == null)
            return;

        AspectRatioFitter fitter = screenshot.GetComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = sprite.rect.width / sprite.rect.height;
    }

    private void OnRowClicked()
    {
        onSelected?.Invoke(this);
    }
}
