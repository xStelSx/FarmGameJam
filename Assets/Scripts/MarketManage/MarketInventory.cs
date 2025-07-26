using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MarketInventory : MonoBehaviour
{
    private InventoryManager inventoryManager;
    public GameObject ItemsPanel;
    public GameObject itemIconPrefab;
    private List<GameObject> itemIcons = new List<GameObject>();

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        if (inventoryManager.Segments.Count > 0)
        {
            foreach (Transform child in ItemsPanel.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < inventoryManager.Segments.Count; i++)
            {
                var segment = inventoryManager.Segments[i];

                GameObject itemIcon = Instantiate(itemIconPrefab, ItemsPanel.transform);

                itemIcons.Add(itemIcon);

                Image iconImage = itemIcon.transform.Find("Icon").GetComponent<Image>();
                Text priceText = itemIcon.transform.Find("Price").GetComponent<Text>();

                ItemIconHandler iconHandler = itemIcon.GetComponent<ItemIconHandler>();

                iconImage.sprite = segment.Icon;
                //priceText.text = segment.Price.ToString(); 

                iconHandler.itemIndex = i;
            }
        }
        
    }
}
