using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadViewController : MonoBehaviour
{
    [SerializeField] private SaveSlotView slotPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject stub;
    [SerializeField] private GameObject saveLoadPanel;

    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    private readonly List<SaveSlotView> slotViews = new();
    private SaveSlotView selectedSlot;

    private void OnEnable()
    {
        RefreshList();
        SaveLoadManager.Instance.OnSaveCompleted += RefreshList;
    }

    private void OnDisable()
    {
        SaveLoadManager.Instance.OnSaveCompleted -= RefreshList;
    }

    private void RefreshList()
    {
        ClearList();

        List<SaveFileData> saves = SaveLoadManager.Instance.GetSavedGames();
        
        foreach (SaveFileData save in saves)
        {
            SaveSlotView slot = Instantiate(slotPrefab, content);
            slot.Initialize(save, SelectSlot);
            slotViews.Add(slot);
        }
        
        SelectSlot(null);
        
        stub.SetActive(saves.Count == 0);
    }
    
    private void ClearList()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    private void SelectSlot(SaveSlotView slot)
    {
        if (selectedSlot != null)
            selectedSlot.SetSelected(false);

        selectedSlot = slot;

        if (selectedSlot != null)
            selectedSlot.SetSelected(true);

        bool hasSelection = selectedSlot != null;

        loadButton.interactable = hasSelection;
        deleteButton.interactable = hasSelection;
    }
    
    public void SaveGame()
    {
        
        
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.ExecuteSave();
        }
    }
    
    public void LoadSelected()
    {
        if (selectedSlot == null)
            return;

        SaveFileData data = selectedSlot.Data;

        Debug.Log("Load save: " + data.SaveFileName);

        if (saveLoadPanel != null)
            saveLoadPanel.SetActive(false);
        
        SaveLoadManager.Instance.ExecuteLoad(data.SaveFileName);
    }

    public void DeleteSelected()
    {
        if (selectedSlot == null)
            return;

        SaveFileData data = selectedSlot.Data;

        Debug.Log("Delete save: " + data.SaveFileName);

        SaveLoadManager.Instance.DeleteSaveFile(data.SaveFileName);

        RefreshList();
    }
}
