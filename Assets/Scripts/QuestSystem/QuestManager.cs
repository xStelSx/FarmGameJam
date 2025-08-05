using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private QuestDataBase questDatabase;
    [SerializeField] private InventorySystem inventorySystem;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questIdText;
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private Button exportButton;

    [Header("Requirements UI")]
    [SerializeField] private Image[] resourceImages; 
    [SerializeField] private TextMeshProUGUI[] resourceTexts; 

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] public Button restartButton;

    [Header("Winner UI")]
    [SerializeField] private GameObject winnerObject;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button winnerButton;
    [SerializeField] private GameObject questsPackage;

    [Header("Other References")]
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private SegmentsLock segmentLock;

    [Header("Attempts UI")]
    public GameObject attempt1;
    public GameObject attempt2;
    public GameObject attempt3;
    public GameObject attempt4;

    [Header("Animation")]
    public GameObject restartPanel;
    public Animator transition;
    public Animator transition2;
    public float animationRestartCooldown = 1f;
    [SerializeField] private float animationDelay = 0.5f;

    public int currentQuestIndex = 0;
    public QuestData currentQuest;
    private QuestProgress currentProgress;
    public int attemptsToComplete = 0;



    private void Awake()
    {
        Time.timeScale = 1f;
        restartPanel.SetActive(true);
        transition.SetTrigger("TriggerRestart2");
        exportButton.onClick.AddListener(CompleteCurrentQuest);
        StartNextQuest();

        if (segmentLock != null)
            segmentLock.CheckCcurrentQuestIndex(currentQuestIndex);

        if (inventorySystem != null)
            inventorySystem.OnInventoryChanged += UpdateQuestUI;
    }

    private void OnDestroy()
    {
        if (inventorySystem != null)
            inventorySystem.OnInventoryChanged -= UpdateQuestUI;
    }

    private void StartNextQuest()
    {
        HideAllRequirements();

        if (currentQuestIndex >= questDatabase.questStructures.Count)
        {
            ShowWinnerScreen();
            return;
        }

        currentQuest = questDatabase.questStructures[currentQuestIndex];
        currentProgress = new QuestProgress(currentQuest, inventorySystem);
        attemptsToComplete = currentQuest.attempsToComplete;

        if (segmentLock != null)
            segmentLock.CheckCcurrentQuestIndex(currentQuestIndex);

        SetupRequirementsUI();
        UpdateQuestUI();
        UpdateAttemptsUI();
    }

    private void HideAllRequirements()
    {
  
        foreach (var image in resourceImages)
            if (image != null) image.gameObject.SetActive(false);

        foreach (var text in resourceTexts)
            if (text != null) text.gameObject.SetActive(false);
    }

    private void SetupRequirementsUI()
    {
        if (currentQuest == null || currentQuest.segmentRequirements == null)
        {
            Debug.LogError("Quest or requirements not set!");
            return;
        }

        int requirementsCount = currentQuest.segmentRequirements.Count;
        int maxToShow = Mathf.Min(requirementsCount, resourceImages.Length, resourceTexts.Length);

        for (int i = 0; i < maxToShow; i++)
        {
            var requirement = currentQuest.segmentRequirements[i];

            
            if (i < resourceImages.Length && resourceImages[i] != null)
            {
                resourceImages[i].gameObject.SetActive(true);
                resourceImages[i].sprite = requirement.segmentImage;
            }

           
            if (i < resourceTexts.Length && resourceTexts[i] != null)
            {
                resourceTexts[i].gameObject.SetActive(true);
                resourceTexts[i].text = $"{inventorySystem.GetItemCount(requirement.segmentId)}/{requirement.quantityResourse}";
            }
        }
    }

    private void UpdateQuestUI()
    {
        if (currentQuest == null) return;

        questIdText.text = currentQuest.questId.ToString();
        attemptsText.text = attemptsToComplete.ToString();

        int requirementsCount = currentQuest.segmentRequirements.Count;
        int maxToShow = Mathf.Min(requirementsCount, resourceTexts.Length);

        for (int i = 0; i < maxToShow; i++)
        {
            var requirement = currentQuest.segmentRequirements[i];
            if (resourceTexts[i] != null)
            {
                int currentAmount = inventorySystem.GetItemCount(requirement.segmentId);
                resourceTexts[i].text = $"{currentAmount}/{requirement.quantityResourse}";
            }
        }
    }

    private void UpdateAttemptsUI()
    {
        if (attempt1 != null) attempt1.SetActive(attemptsToComplete >= 1);
        if (attempt2 != null) attempt2.SetActive(attemptsToComplete >= 2);
        if (attempt3 != null) attempt3.SetActive(attemptsToComplete >= 3);
        if (attempt4 != null) attempt4.SetActive(attemptsToComplete >= 4);
    }

    public void AddProgress(int segmentId, int amount = 1)
    {
        if (currentProgress == null) return;
        currentProgress.AddSegmentProgress(segmentId, amount);
        UpdateQuestUI();
    }

    public void CompleteCurrentQuest()
    {
        if (currentProgress == null || !currentProgress.IsComplete())
        {
            Debug.Log("Не все условия выполнены");
            return;
        }

        foreach (var requirement in currentQuest.segmentRequirements)
            inventorySystem.RemoveItem(requirement.segmentId, requirement.quantityResourse);

        currentQuestIndex++;
        StartNextQuest();
    }

    public bool TryCompleteCurrentQuest()
    {
        if (currentProgress == null)
        {
            Debug.Log("Прогресс не инициализирован");
            return false;
        }

        if (attemptsToComplete <= 0)
        {
            HandleFailedQuest();
            return false;
        }

        if (!currentProgress.IsComplete())
        {
            attemptsToComplete--;
            UpdateQuestUI();
            UpdateAttemptsUI();

            if (attemptsToComplete <= 0)
                StartCoroutine(CarExportTimer());

            return false;
        }

        moneyManager.AddMoney(currentQuest.moneyReward);
        foreach (var requirement in currentQuest.segmentRequirements)
            inventorySystem.RemoveItem(requirement.segmentId, requirement.quantityResourse);

        SoundManager.Instance.Play("QuestComplete");
        currentQuestIndex++;
        StartNextQuest();
        return true;
    }

    private void ShowWinnerScreen()
    {
        if (winnerObject != null)
        {
            questsPackage.SetActive(false);
            transition.SetTrigger("Start");
            winnerObject.SetActive(true);
            winnerButton.gameObject.SetActive(true);
            winnerText.text = currentQuest.questId.ToString();
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartScene);
        }
        StartCoroutine(PlayVictorySoundAfterDelay());
    }

    private void HandleFailedQuest()
    {
        SoundManager.Instance.Play("Lose");
        if (gameOverObject != null)
        {
            transition.SetTrigger("Start");
            gameOverObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            gameOverText.text = currentQuest.questId.ToString();
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartScene);
        }
    }

    private IEnumerator PlayVictorySoundAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.Play("Victory");
    }

    private IEnumerator CarExportTimer()
    {
        yield return new WaitForSeconds(10f);
        CarExportTravel carExport = FindObjectOfType<CarExportTravel>();
        if (carExport != null)
            carExport.StopCarAtCurrentPosition();
        HandleFailedQuest();
    }

    public void RestartScene()
    {
        transition2.SetTrigger("TriggerRestart");
        StartCoroutine(RestartAnimationCooldown());
    }

    private IEnumerator RestartAnimationCooldown()
    {
        transition2.SetTrigger("TriggerRestart");
        yield return new WaitForSeconds(animationRestartCooldown);
        SoundManager.Instance.Play("OnButtonClick");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
                return false;
        }
        return true;
    }
}