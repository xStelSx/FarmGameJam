using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;

    [Header("Item Count Texts")]
    [SerializeField] private TMP_Text item1Text;
    [SerializeField] private TMP_Text item2Text;
    [SerializeField] private TMP_Text item3Text;
    [SerializeField] private TMP_Text item4Text;
    [SerializeField] private TMP_Text item5Text;
    [SerializeField] private TMP_Text item6Text;
    [SerializeField] private TMP_Text item7Text;
    [SerializeField] private TMP_Text item8Text;
    [SerializeField] private TMP_Text item9Text;
    [SerializeField] private TMP_Text item10Text;
    [SerializeField] private TMP_Text item11Text;
    [SerializeField] private TMP_Text item12Text;
    [SerializeField] private TMP_Text currentAmountOfItems;
    [SerializeField] private TMP_Text maxTotalItems;


    [Header("Blink Settings")]
    [SerializeField] private Color normalColor = new Color(77f / 255f, 77f / 255f, 37f / 255f, 1f); 
    [SerializeField] private Color fullColor = Color.red;
    [SerializeField] private float blinkInterval = 1f;

    private bool isBlinking = false;
    private Coroutine blinkCoroutine;
    private bool colorsInitialized = false;

    private void Awake()
    {
        InitializeColors();
    }

    private void InitializeColors()
    {

        currentAmountOfItems.color = normalColor;
        maxTotalItems.color = normalColor;
        colorsInitialized = true;
    }

    private void OnEnable()
    {
        if (!colorsInitialized) InitializeColors();
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
        CheckInventoryFull();
    }

    public void UpdateUI()
    {
        item1Text.text = inventorySystem.item1.ToString();
        item2Text.text = inventorySystem.item2.ToString();
        item3Text.text = inventorySystem.item3.ToString();
        item4Text.text = inventorySystem.item4.ToString();
        item5Text.text = inventorySystem.item5.ToString();
        item6Text.text = inventorySystem.item6.ToString();
        item7Text.text = inventorySystem.item7.ToString();
        item8Text.text = inventorySystem.item8.ToString();
        item9Text.text = inventorySystem.item9.ToString();
        item10Text.text = inventorySystem.item10.ToString();
        item11Text.text = inventorySystem.item11.ToString();
        item12Text.text = inventorySystem.item12.ToString();
        
        currentAmountOfItems.text = inventorySystem.currentTotalItems.ToString();
        maxTotalItems.text = inventorySystem.maxTotalItems.ToString();
    }

    private void CheckInventoryFull()
    {
        bool isFull = inventorySystem.currentTotalItems >= inventorySystem.maxTotalItems;

        if (isFull && !isBlinking)
        {
            isBlinking = true;
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkText());
        }
        else if (!isFull && isBlinking)
        {
            isBlinking = false;
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
            ResetTextColors(true); 
        }
    }

    private IEnumerator BlinkText()
    {
        while (isBlinking)
        {

            SetTextColors(fullColor);
            yield return new WaitForSeconds(blinkInterval);

            SetTextColors(normalColor);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void SetTextColors(Color color)
    {
        if (currentAmountOfItems != null) currentAmountOfItems.color = color;
        if (maxTotalItems != null) maxTotalItems.color = color;
    }

    private void ResetTextColors(bool forceReset = false)
    {
        if (forceReset || !isBlinking)
        {
            SetTextColors(normalColor);

        }
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        isBlinking = false;
        ResetTextColors(true);
    }
}