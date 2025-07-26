using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [Header("Money Settings")]
    [SerializeField] private int currentMoney = 0; 
    [SerializeField] private TextMeshProUGUI moneyText; 

    [Header("Item Prices")]
    [SerializeField] private int[] itemPrices = new int[12]; 

    private InventorySystem inventorySystem;

    private void Awake()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();

        UpdateMoneyUI();
    }


    public void CalculateExportProfit()
    {
        if (inventorySystem == null) return;

        int totalProfit = 0;


        for (int i = 0; i < itemPrices.Length; i++)
        {
            int itemId = i + 1; 
            int itemCount = inventorySystem.GetItemCount(itemId);
            totalProfit += itemCount * itemPrices[i];
        }

        AddMoney(totalProfit);
    }

 
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }


    public bool SubtractMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        else
        {
            Debug.LogWarning("Δενεγ νες!");
            return false;
        }
    }


    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }
    }


    public int GetCurrentMoney() => currentMoney;
}
