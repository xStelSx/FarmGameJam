using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MarketInventory : MonoBehaviour
{
    private ISManager iSManager;
    public GameObject ItemsPanel;
    public GameObject itemIconPrefab;
    public List<GameObject> itemIcons = new List<GameObject>();

    void Start()
    {
        iSManager = FindObjectOfType<ISManager>();
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        if (iSManager.Segments.Count > 0)
        {
            foreach (Transform child in ItemsPanel.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < iSManager.Segments.Count; i++)
            {
                var segment = iSManager.Segments[i];

                GameObject itemIcon = Instantiate(itemIconPrefab, ItemsPanel.transform);

                itemIcons.Add(itemIcon);

                //Image iconImage = itemIcon.transform.Find("Icon").GetComponent<Image>();
                //TMP_Text priceText = itemIcon.transform.Find("Price").GetComponent<TMP_Text>();

                ItemIconHandler iconHandler = itemIcon.GetComponent<ItemIconHandler>();

                //iconImage.sprite = segment.Icon;
                //priceText.text = segment.Price.ToString(); 

                iconHandler.itemIndex = i;
            }
        }
        
    }
}
