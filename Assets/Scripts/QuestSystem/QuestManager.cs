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
    [SerializeField] public Button restartButton;


    [Header("Winner UI")]
    [SerializeField] private GameObject winnerObject;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button winnerButton;
    [SerializeField] private GameObject questsPackage;

    [SerializeField] private MoneyManager moneyManager;
    public int currentQuestIndex = 0;
    public QuestData currentQuest;
    private QuestProgress currentProgress;


    public GameObject restartPanel;

    public Animator transition;
    public Animator transition2;

    public float animationRestartCooldown = 1f;

    [SerializeField] private float animationDelay = 0.5f;


    public int attemptsToComplete = 0;

    public GameObject attempt1;
    public GameObject attempt2;
    public GameObject attempt3;
    public GameObject attempt4;

    public SegmentsLock segmentLock;

    private void Awake()
    {
        Time.timeScale = 1f;
        restartPanel.SetActive(true);
        transition.SetTrigger("TriggerRestart2");
        exportButton.onClick.AddListener(CompleteCurrentQuest);
        StartNextQuest();
        segmentLock.CheckCcurrentQuestIndex(currentQuestIndex);
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
                transition.SetTrigger("Start");

                winnerObject.SetActive(true);
                    winnerButton.gameObject.SetActive(true);
              

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
            StartCoroutine(PlayVictorySoundAfterDelay());
            Debug.Log("All quests completed!");
            return;
        }
        //SoundManager.Instance.Play("NewQuestAdded");
        currentQuest = questDatabase.questStructures[currentQuestIndex];
        currentProgress = new QuestProgress(currentQuest, inventorySystem);
        attemptsToComplete = currentQuest.attempsToComplete;
        segmentLock.CheckCcurrentQuestIndex(currentQuestIndex);
        UpdateQuestUI();

    }

    private IEnumerator PlayVictorySoundAfterDelay()
    {
        // Ждем 15 секунд перед воспроизведением звука
        yield return new WaitForSeconds(2f);

        // Проигрываем звук победы
        SoundManager.Instance.Play("Victory");
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
                StartCoroutine(CarExportTimer());
                //HandleFailedQuest();
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
        SoundManager.Instance.Play("Lose");
        Debug.Log("GAME OVER NAHER");
        // Активируем Game Over объект
        if (gameOverObject != null)
        {
            transition.SetTrigger("Start");
            gameOverObject.SetActive(true);
            restartButton.gameObject.SetActive(true);


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


    public void GameendAnimStart()
    {
        //вот твоя строчка кода для запуска анимации конца игры
        transition.SetTrigger("Start");
    }
    public void RestartScene()
    {
        transition2.SetTrigger("TriggerRestart");
        StartCoroutine(RestartAnimationCooldown());
        //SoundManager.Instance.Play("ButtonClick");
        //UnityEngine.SceneManagement.SceneManager.LoadScene(
        //    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }


    private IEnumerator RestartAnimationCooldown()
    {
        transition2.SetTrigger("TriggerRestart");

        // Ждем завершения анимации
        yield return new WaitForSeconds(animationRestartCooldown);

        // Перезагружаем сцену
        SoundManager.Instance.Play("OnButtonClick");
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

    }

    private IEnumerator CarExportTimer()
    {
        yield return new WaitForSeconds(15f);

        // Получаем компонент CarExportTravel и останавливаем машину
        CarExportTravel carExport = FindObjectOfType<CarExportTravel>();
        if (carExport != null)
        {
            carExport.StopCarAtCurrentPosition();
        }

        HandleFailedQuest();
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

        //МЕНЯ ЗАСТАВИЛИ ЭТО НЕ Я ПИСАЛ!!!!
        if (attemptsToComplete == 4)
        {
            attempt1.SetActive(true);
            attempt2.SetActive(true);
            attempt3.SetActive(true);
            attempt4.SetActive(true);
        }
        if (attemptsToComplete == 3)
        {
            attempt4.SetActive(false);
        }
        if (attemptsToComplete == 2)
        {
            attempt3.SetActive(false);
        }
        if (attemptsToComplete == 1)
        {
            attempt2.SetActive(false);
        }
        if (attemptsToComplete == 0)
        {
            attempt1.SetActive(false);
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




