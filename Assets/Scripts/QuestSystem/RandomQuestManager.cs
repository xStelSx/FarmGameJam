using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class RandomQuestManager : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private QuestDataBase questDatabase;
    [SerializeField] private InventorySystem inventorySystem;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questIdText;
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private Button exportButton;
    [SerializeField] private Image[] resourceImages = new Image[3];
    [SerializeField] private TextMeshProUGUI[] resourceTexts = new TextMeshProUGUI[3];

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

    [Header("Quest Generation Settings")]
    [SerializeField] private int winAfterQuests = 20;
    [SerializeField] private int minRequirements = 1;
    [SerializeField] private int maxRequirements = 3;
    [SerializeField] private int minQuantity = 1;
    [SerializeField] private int maxQuantity = 5;
    [SerializeField] private int baseMoneyReward = 100;
    [SerializeField] private int attemptsPerQuest = 3;

    public int currentQuestIndex = 1;
    public QuestData currentQuest;
    private RandomQuestProgress currentProgress;
    public int attemptsToComplete = 0;

    private List<SegmentRequirement> allPossibleRequirements = new List<SegmentRequirement>();

    private void Awake()
    {
        Time.timeScale = 1f;
        restartPanel.SetActive(true);
        transition.SetTrigger("TriggerRestart2");
        exportButton.onClick.AddListener(CompleteCurrentQuest);

        InitializePossibleRequirements();
        GenerateNewQuest();
        UpdateQuestUI();

        if (segmentLock != null)
            segmentLock.CheckCcurrentQuestIndex(currentQuestIndex);

        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged += OnInventoryChanged;
            // Принудительное обновление UI при старте
            UpdateQuestUI();
        }
    }

    private void OnDestroy()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged -= OnInventoryChanged;
        }
    }

    private void OnInventoryChanged()
    {
        UpdateQuestUI();
    }

    private void InitializePossibleRequirements()
    {
        allPossibleRequirements.Clear();
        var uniqueRequirements = new Dictionary<int, SegmentRequirement>();

        foreach (var quest in questDatabase.questStructures)
        {
            if (quest.segmentRequirements != null)
            {
                foreach (var req in quest.segmentRequirements)
                {
                    if (!uniqueRequirements.ContainsKey(req.segmentId))
                    {
                        uniqueRequirements.Add(req.segmentId, req);
                    }
                }
            }
        }

        allPossibleRequirements.AddRange(uniqueRequirements.Values);

        if (allPossibleRequirements.Count == 0)
        {
            Debug.LogError("В базе данных нет ни одного требования для квестов!");
        }
    }

    private void GenerateNewQuest()
    {
        if (currentQuestIndex > winAfterQuests)
        {
            ShowWinnerScreen();
            return;
        }

        currentQuest = new QuestData
        {
            questId = currentQuestIndex,
            attempsToComplete = attemptsPerQuest,
            moneyReward = CalculateReward(),
            segmentRequirements = GenerateRequirements()
        };

        attemptsToComplete = currentQuest.attempsToComplete;
        currentProgress = new RandomQuestProgress(currentQuest, inventorySystem);

        UpdateUI();
    }

    private List<SegmentRequirement> GenerateRequirements()
    {
        List<SegmentRequirement> requirements = new List<SegmentRequirement>();

        if (allPossibleRequirements.Count == 0)
        {
            Debug.LogWarning("Нет доступных требований для генерации квеста");
            return requirements;
        }

        int requirementsCount = Random.Range(minRequirements, Mathf.Min(maxRequirements + 1, allPossibleRequirements.Count));
        var availableRequirements = new List<SegmentRequirement>(allPossibleRequirements);

        for (int i = 0; i < requirementsCount; i++)
        {
            if (availableRequirements.Count == 0) break;

            int randomIndex = Random.Range(0, availableRequirements.Count);
            SegmentRequirement selected = availableRequirements[randomIndex];

            requirements.Add(new SegmentRequirement
            {
                segmentId = selected.segmentId,
                segmentImage = selected.segmentImage,
                quantityResourse = Random.Range(minQuantity, maxQuantity + 1)
            });

            availableRequirements.RemoveAt(randomIndex);
        }

        return requirements;
    }

    private int CalculateReward()
    {
        int reward = baseMoneyReward;
        foreach (var req in currentQuest.segmentRequirements)
        {
            reward += req.quantityResourse * 10;
        }
        return reward;
    }

    private void UpdateUI()
    {
        // Сначала скрываем все элементы
        for (int i = 0; i < resourceImages.Length; i++)
        {
            if (resourceImages[i] != null)
            {
                resourceImages[i].gameObject.SetActive(false);
            }
            if (i < resourceTexts.Length && resourceTexts[i] != null)
            {
                resourceTexts[i].gameObject.SetActive(false);
            }
        }

        // Затем показываем только нужные
        if (currentQuest?.segmentRequirements != null)
        {
            for (int i = 0; i < currentQuest.segmentRequirements.Count; i++)
            {
                if (i < resourceImages.Length && resourceImages[i] != null)
                {
                    resourceImages[i].gameObject.SetActive(true);
                    resourceImages[i].sprite = currentQuest.segmentRequirements[i].segmentImage;
                }
                if (i < resourceTexts.Length && resourceTexts[i] != null)
                {
                    resourceTexts[i].gameObject.SetActive(true);
                }
            }
        }

        UpdateQuestUI();
        UpdateAttemptsUI();
    }

    private void UpdateQuestUI()
    {
        if (currentQuest == null) return;

        questIdText.text = currentQuest.questId.ToString();
        attemptsText.text = attemptsToComplete.ToString();

        // Обновляем только активные требования
        for (int i = 0; i < currentQuest.segmentRequirements.Count; i++)
        {
            if (i >= resourceTexts.Length || resourceTexts[i] == null) continue;

            var requirement = currentQuest.segmentRequirements[i];
            int currentAmount = inventorySystem.GetItemCount(requirement.segmentId);
            resourceTexts[i].text = $"{currentAmount}/{requirement.quantityResourse}";
        }

        // Скрываем тексты для неактивных требований
        for (int i = currentQuest.segmentRequirements.Count; i < resourceTexts.Length; i++)
        {
            if (resourceTexts[i] != null)
            {
                resourceTexts[i].gameObject.SetActive(false);
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

    public void CompleteCurrentQuest()
    {
        if (currentProgress == null || !currentProgress.IsComplete())
        {
            Debug.Log("Не все условия выполнены");
            return;
        }

        CompleteQuestSuccess();
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

        CompleteQuestSuccess();
        return true;
    }

    private void CompleteQuestSuccess()
    {
        moneyManager.AddMoney(currentQuest.moneyReward);

        foreach (var requirement in currentQuest.segmentRequirements)
        {
            inventorySystem.RemoveItem(requirement.segmentId, requirement.quantityResourse);
        }

        SoundManager.Instance.Play("QuestComplete");

        currentQuestIndex++;
        GenerateNewQuest();

        if (segmentLock != null)
            segmentLock.CheckCcurrentQuestIndex(currentQuestIndex);
    }

    private void ShowWinnerScreen()
    {
        if (winnerObject != null)
        {
            questsPackage.SetActive(false);
            transition.SetTrigger("Start");
            winnerObject.SetActive(true);
            winnerButton.gameObject.SetActive(true);
            winnerText.text = (currentQuestIndex - 1).ToString();
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
            gameOverText.text = (currentQuestIndex - 1).ToString();
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
        var carExport = FindObjectOfType<CarExportTravel>();
        carExport?.StopCarAtCurrentPosition();
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

public class RandomQuestProgress
{
    public QuestData QuestData { get; }
    private readonly InventorySystem _inventory;

    public RandomQuestProgress(QuestData questData, InventorySystem inventory)
    {
        QuestData = questData;
        _inventory = inventory;
    }

    public void AddSegmentProgress(int segmentId, int amount)
    {
        _inventory.AddItem(segmentId, amount);
    }

    public bool IsComplete()
    {
        foreach (var req in QuestData.segmentRequirements)
        {
            if (!_inventory.HasEnoughItems(req.segmentId, req.quantityResourse))
                return false;
        }
        return true;
    }
}