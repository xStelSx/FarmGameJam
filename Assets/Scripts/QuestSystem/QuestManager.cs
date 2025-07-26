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
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private Button exportButton;


    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;


    [Header("Winner UI")]
    [SerializeField] private GameObject winnerObject;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button winnerButton;
    [SerializeField] private GameObject questsPackage;

    [SerializeField] private MoneyManager moneyManager;
    private int currentQuestIndex = 0;
    public QuestData currentQuest;
    private QuestProgress currentProgress;


    [SerializeField] private Animator gameOverAnimator;
    [SerializeField] private float animationDelay = 0.5f;


    public int attemptsToComplete = 0;

    private void Awake()
    {
        exportButton.onClick.AddListener(CompleteCurrentQuest);
        StartNextQuest();

        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged += UpdateQuestUI;
        }
    }

    private void OnDestroy()
    {

        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged -= UpdateQuestUI;
        }
    }

    private void StartNextQuest()
    {
        if (currentQuestIndex >= questDatabase.questStructures.Count)
        {
            if (winnerObject != null)
            {
                questsPackage.SetActive(false);

                if (gameOverAnimator != null)
                {
                    winnerObject.SetActive(true);
                    gameOverAnimator.SetTrigger("Show");
                    StartCoroutine(EnableButtonAfterAnimation(winnerButton, gameOverAnimator));
                }
                else
                {
                    winnerObject.SetActive(true);
                    winnerButton.gameObject.SetActive(true);
                }

                if (winnerText != null)
                {
                    winnerText.text = currentQuest.questId.ToString();

                    foreach (var req in currentQuest.segmentRequirements)
                    {
                        int currentAmount = inventorySystem.GetItemCount(req.segmentId);
                        //gameOverText.text += $"- Item {req.segmentId}: {currentAmount}/{req.quantityResourse}\n";
                    }
                }


                if (restartButton != null)
                {
                    restartButton.onClick.RemoveAllListeners();
                    restartButton.onClick.AddListener(RestartScene);
                }
            }
            Debug.Log("All quests completed!");
            return;
        }
        //SoundManager.Instance.Play("NewQuestAdded");
        currentQuest = questDatabase.questStructures[currentQuestIndex];
        currentProgress = new QuestProgress(currentQuest, inventorySystem);
        attemptsToComplete = currentQuest.attempsToComplete;
        UpdateQuestUI();
    }

    private void UpdateQuestUI()
    {
        if (currentQuest == null || currentQuest.segmentRequirements.Count == 0) return;

        var firstSegment = currentQuest.segmentRequirements[0];
        questIdText.text = currentQuest.questId.ToString();
        resourceImage.sprite = firstSegment.segmentImage;
        int currentAmount = inventorySystem.GetItemCount(firstSegment.segmentId);
        resourceAmountText.text = $"{inventorySystem.GetItemCount(firstSegment.segmentId)}/{firstSegment.quantityResourse}";
        attemptsText.text = attemptsToComplete.ToString();
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

    private IEnumerator EnableButtonAfterAnimation(Button button, Animator animator)
    {
        // Ждем пока анимация закончится
        yield return new WaitForSeconds(animationDelay);

        // Дополнительная проверка, если анимация длиннее
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        button.gameObject.SetActive(true);
    }

    public void CompleteCurrentQuest()
    {
        if (currentProgress == null) return;

        if (!currentProgress.IsComplete())
        {
            Debug.Log("не сделал условие");
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
            Debug.Log("квесты кончились");
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

            Debug.Log("не сделал условие");
            if (attemptsToComplete <= 0)
            {
                HandleFailedQuest();
            }
            return false;
            //Debug.Log("Quest requirements not met! Current progress:");
            //foreach (var req in currentQuest.segmentRequirements)
            //{
            //    int amount = inventorySystem.GetItemCount(req.segmentId);
            //    Debug.Log($"Item {req.segmentId}: {amount}/{req.quantityResourse}");
            //}
            //return false;
        }

        moneyManager.AddMoney(currentQuest.moneyReward);
        foreach (var requirement in currentQuest.segmentRequirements)
        {
            inventorySystem.RemoveItem(requirement.segmentId, requirement.quantityResourse);
        }
        SoundManager.Instance.Play("QuestComplete");
        currentQuestIndex++;
        StartNextQuest();
        Debug.Log("молодец сделал!");
        return true;
    }

    private void HandleFailedQuest()
    {
        Debug.Log("GAME OVER NAHER");

        // Активируем Game Over объект
        if (gameOverObject != null)
        {
            if (gameOverAnimator != null)
            {
                gameOverObject.SetActive(true);
                gameOverAnimator.SetTrigger("Show");
                StartCoroutine(EnableButtonAfterAnimation(restartButton, gameOverAnimator));
            }
            else
            {
                gameOverObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
            }

            // Устанавливаем текст с информацией о текущем квесте
            if (gameOverText != null)
            {
                gameOverText.text = currentQuest.questId.ToString();

                foreach (var req in currentQuest.segmentRequirements)
                {
                    int currentAmount = inventorySystem.GetItemCount(req.segmentId);
                    //gameOverText.text += $"- Item {req.segmentId}: {currentAmount}/{req.quantityResourse}\n";
                }
            }

            // Активируем кнопку рестарта
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RestartScene);
            }
        }
    }

    public void RestartScene()
    {
        SoundManager.Instance.Play("ButtonClick");
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (currentQuest != null && currentQuest.segmentRequirements.Count > 0)
        //    {
        //        AddProgress(currentQuest.segmentRequirements[0].segmentId, 1);
        //    }
        //}
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




