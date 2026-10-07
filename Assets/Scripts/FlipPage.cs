using System;
using System.Collections.Generic;
using UnityEngine;

// --- SAVE DATA CONTAINERS ---
[Serializable]
public class CardSaveData
{
    public string cardPrefabName; // Name of prefab in Resources/Cards
    public int spreadIndex;       // Which page spread (0, 1, 2...)
    public int slotIndex;         // Which slot on that page (0 to 7)
}

[Serializable]
public class BinderSaveData
{
    public List<CardSaveData> savedCards = new List<CardSaveData>();
}

// --- BINDER MANAGER CLASS ---
public class FlipPage : MonoBehaviour
{
    [Header("Binder Spreads")]
    [SerializeField] private List<GameObject> pageSpreads = new List<GameObject>();

    [Header("Binder Reference")]
    [SerializeField] private Transform binderTransform;

    [Header("Save Settings")]
    [SerializeField] private string saveKey = "BinderSaveData_v1";

    private int currentSpreadIndex = 0;

    private void Start()
    {
        if (binderTransform == null)
        {
            binderTransform = transform;
        }

        UpdatePageVisibility();

        // Load saved state automatically on game start
        LoadBinderProgress();
    }

    private void OnApplicationQuit()
    {
        // Save automatically when exiting game
        SaveBinderProgress();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearBinderSave();
        }
        // Right Mouse Button (1) click on 3D binder model
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.IsChildOf(binderTransform) || hit.transform == binderTransform)
                {
                    Vector3 localHit = binderTransform.InverseTransformPoint(hit.point);

                    if (localHit.x > 0)
                    {
                        NextPage();
                    }
                    else
                    {
                        PreviousPage();
                    }
                }
            }
        }
    }

    [ContextMenu("Clear Save Data")]
    public void ClearBinderSave()
    {
        // 1. Delete saved string from PlayerPrefs
        if (PlayerPrefs.HasKey(saveKey))
        {
            PlayerPrefs.DeleteKey(saveKey);
            PlayerPrefs.Save();
            Debug.Log("🗑️ Binder save data cleared from PlayerPrefs!");
        }

        // 2. Clear all cards currently in slots in the active scene
        for (int spreadIdx = 0; spreadIdx < pageSpreads.Count; spreadIdx++)
        {
            PutInBinder[] slots = pageSpreads[spreadIdx].GetComponentsInChildren<PutInBinder>(true);

            foreach (PutInBinder slot in slots)
            {
                if (slot.CurrentCardInSlot != null)
                {
                    Destroy(slot.CurrentCardInSlot);
                    // Clear the slot reference using OnMouseDown take logic or direct setting
                }
            }
        }
    }

    public void NextPage()
    {
        if (currentSpreadIndex < pageSpreads.Count - 1)
        {
            currentSpreadIndex++;
            UpdatePageVisibility();
        }
    }

    public void PreviousPage()
    {
        if (currentSpreadIndex > 0)
        {
            currentSpreadIndex--;
            UpdatePageVisibility();
        }
    }

    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pageSpreads.Count; i++)
        {
            pageSpreads[i].SetActive(i == currentSpreadIndex);
        }
    }

    // --- SAVE METHOD ---
    public void SaveBinderProgress()
    {
        BinderSaveData saveData = new BinderSaveData();

        for (int spreadIdx = 0; spreadIdx < pageSpreads.Count; spreadIdx++)
        {
            PutInBinder[] slots = pageSpreads[spreadIdx].GetComponentsInChildren<PutInBinder>(true);

            for (int slotIdx = 0; slotIdx < slots.Length; slotIdx++)
            {
                GameObject card = slots[slotIdx].CurrentCardInSlot;

                if (card != null)
                {
                    CardSaveData cardData = new CardSaveData
                    {
                        // Removes "(Clone)" string if card was instantiated
                        cardPrefabName = card.name.Replace("(Clone)", "").Trim(),
                        spreadIndex = spreadIdx,
                        slotIndex = slotIdx
                    };
                    saveData.savedCards.Add(cardData);
                }
            }
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Binder state saved!");
    }

    // --- LOAD METHOD ---
    public void LoadBinderProgress()
    {
        if (!PlayerPrefs.HasKey(saveKey)) return;

        string json = PlayerPrefs.GetString(saveKey);
        BinderSaveData saveData = JsonUtility.FromJson<BinderSaveData>(json);

        // 1. Temporarily activate ALL page spreads so slot transform positions are valid in 3D space
        for (int i = 0; i < pageSpreads.Count; i++)
        {
            pageSpreads[i].SetActive(true);
        }

        foreach (CardSaveData cardData in saveData.savedCards)
        {
            // Check bounds on spread index
            if (cardData.spreadIndex >= pageSpreads.Count) continue;

            GameObject cardPrefab = Resources.Load<GameObject>("Cards/" + cardData.cardPrefabName);

            if (cardPrefab != null)
            {
                // Spawn card directly as child or at origin
                GameObject newCard = Instantiate(cardPrefab);

                PutInBinder[] slots = pageSpreads[cardData.spreadIndex].GetComponentsInChildren<PutInBinder>(true);

                if (cardData.slotIndex < slots.Length)
                {
                    PutInBinder targetSlot = slots[cardData.slotIndex];

                    if (newCard.TryGetComponent(out TakeCard cardScript))
                    {
                        targetSlot.AssignCardToSlot(newCard, cardScript);
                    }
                }
                else
                {
                    // Slot index out of bounds fallback
                    Destroy(newCard);
                }
            }
            else
            {
                Debug.LogWarning($"Card prefab '{cardData.cardPrefabName}' not found in Resources/Cards/");
            }
        }

        // 2. Now apply the correct visibility so only current page spread is active
        UpdatePageVisibility();

        Debug.Log("Binder state loaded!");
    }
}