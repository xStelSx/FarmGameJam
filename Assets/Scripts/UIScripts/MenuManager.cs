using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject PausePanel;
    public GlobalTimer globalTimer;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnPauseButtonClick()
    {
        if(PausePanel != null) 
        {
        PausePanel.SetActive(true);
        }
        globalTimer.isRunning = false;
        Time.timeScale = 0f;
    }

    public void OnContinueButtonClick()
    {
        if (PausePanel != null)
        {
            PausePanel.SetActive(false);
        }
        Time.timeScale = 1f;
        globalTimer.isRunning = true;
        
    }
}

