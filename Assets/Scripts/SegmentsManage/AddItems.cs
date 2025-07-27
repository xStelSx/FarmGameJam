using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItems : MonoBehaviour
{

    private InventorySystem inventorySystem;

    public int CountItems;
    public int Timer;


    private int itemCount = 0; // Переменная для хранения количества предметов
    void Start()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();
    }

    public void activeCollectionItem(int ID)
    {
        inventorySystem = FindObjectOfType<InventorySystem>();
        StartCoroutine(AddItemCoroutine(ID));
        
    }

    private IEnumerator AddItemCoroutine(int ID)
    {
        while (true) 
        {
            inventorySystem.AddItem(ID + 1, CountItems); 
            yield return new WaitForSeconds(Timer); 
        }
    }

}
