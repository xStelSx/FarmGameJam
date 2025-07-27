using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIconHandler : MonoBehaviour
{
    public int itemIndex;
    //public MarketInventory marketInventory;
    private PlaceManager placeManager;

    private MoneyManager moneyManager;
    private ISManager iSManager;

    void Start()
    {
        //marketInventory = FindObjectOfType<MarketInventory>();
        placeManager = FindObjectOfType<PlaceManager>();
        moneyManager = FindObjectOfType<MoneyManager>();
        iSManager = FindObjectOfType<ISManager>();
    }

    public void OnItemIconClick()
    {
        InterfaceSegments segment = iSManager.Segments[itemIndex];

        if (segment.Price <= moneyManager.currentMoney)
        {

            moneyManager.SubtractMoney(segment.Price);

            placeManager.DetectActive_SegmentOnMarket(itemIndex);

            Debug.Log($"Сегмент {itemIndex} куплен за {segment.Price}!");
        }
        else
        {
            Debug.Log("Недостаточно денег для покупки!");
        }
    }

    public void Delete()
    {
        if(placeManager.segmentArray[placeManager.ID_SegmentOnPlace] != null)
        {
            if (50 <= moneyManager.currentMoney)
            {

                moneyManager.SubtractMoney(50);

                placeManager.DeleteSegment();

                Debug.Log($"Сегмент {itemIndex} куплен за 50!");
            }
            else
            {
                Debug.Log("Недостаточно денег для покупки!");
            }
        }
    }
}