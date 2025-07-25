using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalTimer : MonoBehaviour
{

    //[SerializeField] private TextMeshProUGUI globalTimerText;
    [SerializeField] private float startTimerTime;
    [SerializeField] public bool isRunning = true;
    [SerializeField] private float currentTimerTime;
    [SerializeField] private Button exportButton;
    [SerializeField] private float exportCooldown = 10f;
    [SerializeField] private GameObject PausePanel;

    private bool isExportOnCooldown = false;

    // Start is called before the first frame update
    void Start()
    {

        //ïîäïèñàòü ðåñóðû ê êíîïêå
        startTimerTime = Time.time;

        exportButton.onClick.AddListener(ExportButtonClick);

    }

    // Update is called once per frame
    void Update()
    {
        currentTimerTime = Time.time - startTimerTime;
        int timerMinutes = (int) currentTimerTime / 60;
        int timerSeconds = (int) currentTimerTime % 60;


        //globalTimerText.text = $"{timerMinutes:00}:{timerSeconds:00}";


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

        exportButton.gameObject.SetActive(false);
        isExportOnCooldown = true;

        StartCoroutine(ExportCooldownRoutine());
    }

    private IEnumerator ExportCooldownRoutine()
    {
        yield return new WaitForSeconds(exportCooldown);
        exportButton.gameObject.SetActive(true);
        isExportOnCooldown = false;

    }
}
