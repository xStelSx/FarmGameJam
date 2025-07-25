using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI globalTimerText;
    [SerializeField] private float startTimerTime;
    [SerializeField] private bool isRunning = true;
    [SerializeField] private float currentTimerTime;
    [SerializeField] private Button dispatchButton;
    [SerializeField] private float dispatchCooldown = 10f;

    private bool isDispatchOnCooldown = false;
    // Start is called before the first frame update
    void Start()
    {

        //подписать ресуры к кнопке
        startTimerTime = Time.time;
        dispatchButton.onClick.AddListener(DispatchButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        currentTimerTime = Time.time - startTimerTime;
        int timerMinutes = (int) currentTimerTime / 60;
        int timerSeconds = (int) currentTimerTime % 60;

        globalTimerText.text = $"{timerMinutes:00}:{timerSeconds:00}";

    }

    public void StopTimer()
    {
        isRunning = false;
        Time.timeScale = 0f;
    }

    public void ResumeTimer()
    {
        Time.timeScale = 1f;
        isRunning = true;   
    }

    public void DispatchButtonClick()
    {
        if (isDispatchOnCooldown) return;

        dispatchButton.gameObject.SetActive(false);
        isDispatchOnCooldown = true;

        StartCoroutine(DispatchCooldownRoutine());
    }

    private IEnumerator DispatchCooldownRoutine()
    {
        yield return new WaitForSeconds(dispatchCooldown);
        dispatchButton.gameObject.SetActive(true);
        isDispatchOnCooldown = false;
    }
}
