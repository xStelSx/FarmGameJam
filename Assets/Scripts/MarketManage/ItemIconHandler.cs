using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIconHandler : MonoBehaviour
{
    public int itemIndex;
    //public MarketInventory marketInventory;
    private PlaceManager placeManager;

    void Start()
    {
        //marketInventory = FindObjectOfType<MarketInventory>();
        placeManager = FindObjectOfType<PlaceManager>();
    }

    public void OnItemIconClick()
    {
        placeManager.DetectActive_SegmentOnMarket(itemIndex);
    }

    public void Delete()
    {
        placeManager.DeleteSegment();
    }
}