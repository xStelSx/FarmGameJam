using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject PausePanel;

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
    }

    public void OnContinueButtonClick()
    {
        if (PausePanel != null)
        {
            PausePanel.SetActive(false);
        }
    }
}

