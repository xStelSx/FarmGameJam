using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int maxTotalItems = 100;

    [Header("Item Quantities")]
    public int item1 = 0;
    public int item2 = 0;
    public int item3 = 0;
    public int item4 = 0;
    public int item5 = 0;
    public int item6 = 0;
    public int item7 = 0;
    public int item8 = 0;
    public int item9 = 0;
    public int item10 = 0;
    public int item11 = 0;
    public int item12 = 0;

    private Dictionary<int, int> itemQuantities = new Dictionary<int, int>();
    public int currentTotalItems = 0;

    [SerializeField] private Button exportButton;
    //[SerializeField] private GlobalTimer globalTimer;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        itemQuantities.Clear();
        for (int i = 1; i <= 12; i++)
        {
            itemQuantities.Add(i, 0);
        }
        SyncDictionaryWithUI();
    }

    public void SyncDictionaryWithUI()
    {
        itemQuantities[1] = item1;
        itemQuantities[2] = item2;
        itemQuantities[3] = item3;
        itemQuantities[4] = item4;
        itemQuantities[5] = item5;
        itemQuantities[6] = item6;
        itemQuantities[7] = item7;
        itemQuantities[8] = item8;
        itemQuantities[9] = item9;
        itemQuantities[10] = item10;
        itemQuantities[11] = item11;
        itemQuantities[12] = item12;

        CalculateTotalItems();
        OnInventoryChanged?.Invoke();
    }

    private void SyncUIWithDictionary()
    {
        item1 = itemQuantities[1];
        item2 = itemQuantities[2];
        item3 = itemQuantities[3];
        item4 = itemQuantities[4];
        item5 = itemQuantities[5];
        item6 = itemQuantities[6];
        item7 = itemQuantities[7];
        item8 = itemQuantities[8];
        item9 = itemQuantities[9];
        item10 = itemQuantities[10];
        item11 = itemQuantities[11];
        item12 = itemQuantities[12];
    }

    private void CalculateTotalItems()
    {
        currentTotalItems = 0;
        foreach (var item in itemQuantities)
        {
            currentTotalItems += item.Value;
        }
    }

    public bool AddItem(int segmentId, int amount)
    {
        if (segmentId < 1 || segmentId > 12)
        {
            return false;
        }

        if (currentTotalItems + amount > maxTotalItems)
        {
            return false;
        }

        itemQuantities[segmentId] += amount;
        currentTotalItems += amount;
        SyncUIWithDictionary();
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(int segmentId, int amount)
    {
        if (segmentId < 1 || segmentId > 12)
        {  
            return false;
        }

        if (itemQuantities[segmentId] < amount)
        {
            return false;
        }

        itemQuantities[segmentId] -= amount;
        currentTotalItems -= amount;
        SyncUIWithDictionary();
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetItemCount(int segmentId)
    {
        if (segmentId < 1 || segmentId > 12) return 0;
        return itemQuantities[segmentId];
    }

    public bool HasEnoughItems(int segmentId, int requiredAmount)
    {
        return GetItemCount(segmentId) >= requiredAmount;
    }

    public int GetCurrentTotalItems()
    {
        return currentTotalItems;
    }

    public int GetMaxTotalItems()
    {
        return maxTotalItems;
    }

    //public void HideExportButton()
    //{
    //    if (currentTotalItems < 1 || globalTimer.isExportOnCooldown)
    //    {
    //        exportButton.gameObject.SetActive(false);
    //    }

    //    if (currentTotalItems > 0 && !globalTimer.isExportOnCooldown)
    //    {
    //        exportButton.gameObject.SetActive(true);
    //    }
    //}

    // private void Update()
    // {
    //     for (KeyCode key = KeyCode.Alpha1; key <= KeyCode.Alpha9; key++)
    //     {
    //         if (Input.GetKeyDown(key))
    //         {
    //             int segmentId = (int)key - (int)KeyCode.Alpha1 + 1;
    //             AddItem(segmentId, 1);
    //         }
    //     }

    //     if (Input.GetKeyDown(KeyCode.Q)) AddItem(1, 1);
    //     if (Input.GetKeyDown(KeyCode.W)) AddItem(2, 1);
    //     if (Input.GetKeyDown(KeyCode.E)) AddItem(3, 1);
    //     if (Input.GetKeyDown(KeyCode.R)) AddItem(4, 1);
    //     if (Input.GetKeyDown(KeyCode.T)) AddItem(5, 1);
    //     if (Input.GetKeyDown(KeyCode.Y)) AddItem(6, 1);
    //     if (Input.GetKeyDown(KeyCode.U)) AddItem(7, 1);
    //     if (Input.GetKeyDown(KeyCode.I)) AddItem(8, 1);
    //     if (Input.GetKeyDown(KeyCode.O)) AddItem(9, 1);
    //     if (Input.GetKeyDown(KeyCode.A)) AddItem(10, 1);
    //     if (Input.GetKeyDown(KeyCode.S)) AddItem(11, 1);
    //     if (Input.GetKeyDown(KeyCode.D)) AddItem(12, 1);

    //     if(currentTotalItems == 0)
    //     {
    //         exportButton.interactable = false;
    //     }
    //     else
    //     {
    //         exportButton.interactable = true;
    //     }
    // }
}