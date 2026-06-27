using System;
using System.Collections;
using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadViewController : MonoBehaviour
{
    public enum SaveLoadMode
    {
        LoadOnly,
        SaveLoad
    }
    
    [SerializeField] private SaveLoadMode mode;
    [SerializeField] private SaveSlotView slotPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject stub;
    [SerializeField] private GameObject saveLoadPanel;

    [SerializeField] private GameObject saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    
    private readonly List<SaveSlotView> slotViews = new();
    private SaveSlotView selectedSlot;

    private Coroutine thumbnailRoutine;
    
    private void OnEnable()
    {
        RefreshList();
        SaveLoadManager.Instance.OnSaveCompleted += RefreshList;
    }

    private void OnDisable()
    {
        SaveLoadManager.Instance.OnSaveCompleted -= RefreshList;
    }

    private void Start()
    {
        if (saveButton != null)
        {
            if (mode == SaveLoadMode.LoadOnly)
            {
                saveButton.SetActive(false);
            }
            else
            {
                saveButton.SetActive(true);
            }
        }
    }

    private void RefreshList()
    {
        if (thumbnailRoutine != null)
        {
            StopCoroutine(thumbnailRoutine);
            thumbnailRoutine = null;
        }
        
        ClearList();

        List<SaveFileData> saves = SaveLoadManager.Instance.GetSavedGames();
        
        foreach (SaveFileData save in saves)
        {
            SaveSlotView slot = Instantiate(slotPrefab, content);
            slot.Initialize(save, SelectSlot);
            slotViews.Add(slot);
        }
        
        SelectSlot(null);
        
        if (saves.Count > 0)
        {
            thumbnailRoutine = StartCoroutine(LoadThumbnailsRoutine());
        }
        
        stub.SetActive(saves.Count == 0);
    }
    
    private IEnumerator LoadThumbnailsRoutine()
    {
        foreach (SaveSlotView slot in slotViews)
        {
            if (slot != null)
                slot.LoadThumbnail();

            yield return null;
        }

        thumbnailRoutine = null;
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
        
        GameplaySaveData saveData = SaveLoadManager.Instance.ExecuteLoad(data.SaveFileName);
        SaveLoadManager.Instance.ExecuteLoad(data.SaveFileName);
        
        if (saveLoadPanel != null)
            saveLoadPanel.SetActive(false);
        
        if (mode == SaveLoadMode.SaveLoad)
        {
            GameplayInitializer loader = FindFirstObjectByType<GameplayInitializer>();
            loader.ApplySave(saveData);
        }
        else if (mode == SaveLoadMode.LoadOnly)
        {
            GameManager.Instance.SetPendingSave(saveData);
            SceneManager.LoadScene(1);
        }
        
        Debug.Log("Load save: " + data.SaveFileName);
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
