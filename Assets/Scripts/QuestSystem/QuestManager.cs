using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;

public class QuestManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private QuestDataBase questDatabase;
    [SerializeField] private InventorySystem inventorySystem;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questIdText;
    [SerializeField] private Image resourceImage;
    [SerializeField] private TextMeshProUGUI resourceAmountText;
    [SerializeField] private Button exportButton;

    private int currentQuestIndex = 0;
    private QuestData currentQuest;
    private QuestProgress currentProgress;

    private void Awake()
    {
        exportButton.onClick.AddListener(CompleteCurrentQuest);
        StartNextQuest();
    }

    private void StartNextQuest()
    {
        if (currentQuestIndex >= questDatabase.questStructures.Count)
        {
            Debug.Log("All quests completed!");
            return;
        }

        currentQuest = questDatabase.questStructures[currentQuestIndex];
        currentProgress = new QuestProgress(currentQuest, inventorySystem);
        UpdateQuestUI();
    }

    private void UpdateQuestUI()
    {
        if (currentQuest == null || currentQuest.segmentRequirements.Count == 0) return;

        var firstSegment = currentQuest.segmentRequirements[0];
        questIdText.text = currentQuest.questId.ToString();
        resourceImage.sprite = firstSegment.segmentImage;
        resourceAmountText.text = $"{inventorySystem.GetItemCount(firstSegment.segmentId)}/{firstSegment.quantityResourse}";
    }

    public void AddProgress(int segmentId, int amount = 1)
    {
        if (currentProgress == null) return;
        currentProgress.AddSegmentProgress(segmentId, amount);
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        if (currentQuest == null || currentQuest.segmentRequirements.Count == 0) return;
        var firstSegment = currentQuest.segmentRequirements[0];
        resourceAmountText.text = $"{inventorySystem.GetItemCount(firstSegment.segmentId)}/{firstSegment.quantityResourse}";
    }

    public void CompleteCurrentQuest()
    {
        if (currentProgress == null) return;

        if (!currentProgress.IsComplete())
        {
            Debug.Log("Quest requirements not met!");
            return;
        }

        foreach (var requirement in currentQuest.segmentRequirements)
        {
            inventorySystem.RemoveItem(requirement.segmentId, requirement.quantityResourse);
        }

        currentQuestIndex++;
        StartNextQuest();
    }


    public bool TryCompleteCurrentQuest()
    {
        if (currentProgress == null)
        {
            Debug.Log("No active quest!");
            return false;
        }

        if (!currentProgress.IsComplete())
        {
            Debug.Log("Quest requirements not met! Current progress:");
            foreach (var req in currentQuest.segmentRequirements)
            {
                int amount = inventorySystem.GetItemCount(req.segmentId);
                Debug.Log($"Item {req.segmentId}: {amount}/{req.quantityResourse}");
            }
            return false;
        }

        // Удаляем только нужные для квеста предметы
        foreach (var requirement in currentQuest.segmentRequirements)
        {
            inventorySystem.RemoveItem(requirement.segmentId, requirement.quantityResourse);
        }

        currentQuestIndex++;
        StartNextQuest();
        Debug.Log("Quest completed successfully!");
        return true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentQuest != null && currentQuest.segmentRequirements.Count > 0)
            {
                AddProgress(currentQuest.segmentRequirements[0].segmentId, 1);
            }
        }
    }
}

public class QuestProgress
{
    public QuestData QuestData { get; private set; }
    private InventorySystem inventory;

    public QuestProgress(QuestData questData, InventorySystem inventorySystem)
    {
        QuestData = questData;
        inventory = inventorySystem;
    }

    public void AddSegmentProgress(int segmentId, int amount)
    {
        inventory.AddItem(segmentId, amount);
    }

    public bool IsComplete()
    {
        foreach (var requirement in QuestData.segmentRequirements)
        {
            if (!inventory.HasEnoughItems(requirement.segmentId, requirement.quantityResourse))
            {
                return false;
            }
        }
        return true;
    }
}




