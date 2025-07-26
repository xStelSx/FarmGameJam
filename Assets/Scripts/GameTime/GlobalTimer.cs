using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalTimer : MonoBehaviour
{

    [SerializeField] private float startTimerTime;
    [SerializeField] public bool isRunning = true;
    [SerializeField] private float currentTimerTime;
    [SerializeField] public Button exportButton;
    [SerializeField] public float exportCooldown = 10f;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] public InventorySystem inventorySystem;

    [SerializeField] public QuestManager questManager;

    [SerializeField] private MoneyManager moneyManager;

    public bool isExportOnCooldown = false;


    void Start()
    {

        startTimerTime = Time.time;

        exportButton.onClick.AddListener(ExportButtonClick);

    }


    void Update()
    {
        currentTimerTime = Time.time - startTimerTime;
        int timerMinutes = (int)currentTimerTime / 60;
        int timerSeconds = (int)currentTimerTime % 60;

    }

    public void StopTimer()
    {

        if (PausePanel != null)
        {
            PausePanel.SetActive(true);
        }

        isRunning = false;
        Time.timeScale = 0f;
    }

    public void ResumeTimer()
    {

        if (PausePanel != null)
        {
            PausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isRunning = true;
    }


    public void ExportButtonClick()
    {
        if (isExportOnCooldown) return;

        bool questCompleted = questManager.TryCompleteCurrentQuest();


        //if (questCompleted)
        //{
        moneyManager.CalculateExportProfit();
        exportButton.gameObject.SetActive(false);
        isExportOnCooldown = true;
        StartCoroutine(ExportCooldownRoutine());
        //}

        ClearInventory();
    }

    private IEnumerator ExportCooldownRoutine()
    {
        yield return new WaitForSeconds(exportCooldown);
        exportButton.gameObject.SetActive(true);
        isExportOnCooldown = false;

    }

    private void ClearInventory()
    {
        if (inventorySystem != null)
        {

            inventorySystem.item1 = 0;
            inventorySystem.item2 = 0;
            inventorySystem.item3 = 0;
            inventorySystem.item4 = 0;
            inventorySystem.item5 = 0;
            inventorySystem.item6 = 0;
            inventorySystem.item7 = 0;
            inventorySystem.item8 = 0;
            inventorySystem.item9 = 0;
            inventorySystem.item10 = 0;
            inventorySystem.item11 = 0;
            inventorySystem.item12 = 0;


            inventorySystem.SyncDictionaryWithUI();
        }
        else
        {
            
        }
    }
}