using System;
using System.IO;
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
    
    private Sprite loadedThumbnail;

    public SaveFileData Data => data;
    
    public void Initialize(SaveFileData saveData, Action<SaveSlotView> selectedCallback)
    {
        data = saveData;
        onSelected = selectedCallback;

        string formattedDate = saveData.Date.ToString("dd.MM.yyyy HH:mm");
        saveNameText.text = data.SaveName + " \n" + formattedDate;

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
    
    public void LoadThumbnail()
    {
        if (data == null)
            return;

        if (string.IsNullOrEmpty(data.ThumbnailPath))
            return;
        
        if (!File.Exists(data.ThumbnailPath))
            return;

        byte[] bytes = File.ReadAllBytes(data.ThumbnailPath);

        Texture2D texture = new Texture2D(2, 2);
        if (!texture.LoadImage(bytes))
        {
            Destroy(texture);
            return;
        }

        loadedThumbnail = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        SetThumbnail(loadedThumbnail);
    }

    private void OnDestroy()
    {
        if (loadedThumbnail != null)
        {
            Destroy(loadedThumbnail.texture);
            Destroy(loadedThumbnail);
        }
    }
}
