using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceManager : MonoBehaviour
{

    private MarketManager marketManager;

    private int ID_SegmentOnPlace;
    private int ID_SegmentOnMarket;

    public GameObject[] SegmentsPrefabPlace= new GameObject[6];

    private int?[] segmentArray = new int?[6];

    void Start()
    {
        marketManager = FindObjectOfType<MarketManager>();
    }

    public void ReceiveMessage(int id)
    {
        MarketReport();
        DetectActive_SegmentOnPlace(id);
    }

    public void MarketReport()
    {
        marketManager.OpenMarket();
    }



    public void DetectActive_SegmentOnPlace(int id_SegmentOnPlace)
    {
        ID_SegmentOnPlace = id_SegmentOnPlace;
    }

    public void DetectActive_SegmentOnMarket(int id_SegmentOnMarket)
    {
        ID_SegmentOnMarket = id_SegmentOnMarket;
        FillingPlace();
    }

    public void FillingPlace()
    {
        if (ID_SegmentOnPlace >= 0 && ID_SegmentOnPlace < segmentArray.Length)
        {
            if (segmentArray[ID_SegmentOnPlace] == null)
            {
                segmentArray[ID_SegmentOnPlace] = ID_SegmentOnMarket;
                Debug.Log($"на клетку {ID_SegmentOnPlace} установлено поле {ID_SegmentOnMarket}");
                Replacement();
            }
            else
                Debug.Log($"клетка {ID_SegmentOnPlace} занята полем {segmentArray[ID_SegmentOnPlace]}");

        }
    }

    public void DeleteSegment()
    {

        if (segmentArray[ID_SegmentOnPlace] != null)
        {
            segmentArray[ID_SegmentOnPlace] = null;
            Debug.Log($"клетка {ID_SegmentOnPlace} стала свободной");
        }
        else
            Debug.Log($"клетка {ID_SegmentOnPlace} уже свободна");

    }

    public void Replacement()
    {
        // Проверяем, что индекс находится в пределах массива
        if (ID_SegmentOnPlace >= 0 && ID_SegmentOnPlace < SegmentsPrefabPlace.Length)
        {
            // Проверяем, что сегмент на месте не пуст
            if (segmentArray[ID_SegmentOnPlace] != null)
            {
                // Удаляем старый префаб, если он существует
                if (SegmentsPrefabPlace[ID_SegmentOnPlace] != null)
                {
                    Destroy(SegmentsPrefabPlace[ID_SegmentOnPlace]);
                }
            }
            // Получаем префаб из списка Segments по ID_SegmentOnMarket
            InterfaceSegments segment = FindObjectOfType<InventoryManager>().Segments[ID_SegmentOnMarket];
            // Создаем новый префаб на позиции ID_SegmentOnPlace
            GameObject newSegmentPrefab = Instantiate(segment.Prefab, SegmentsPrefabPlace[ID_SegmentOnPlace].transform.position, Quaternion.identity);
            // Сохраняем новый префаб в массиве
            SegmentsPrefabPlace[ID_SegmentOnPlace] = newSegmentPrefab;
            // Обновляем массив segmentArray
            segmentArray[ID_SegmentOnPlace] = ID_SegmentOnMarket;
            Debug.Log($"На клетку {ID_SegmentOnPlace} установлен новый префаб {ID_SegmentOnMarket}");
        }
        else
        {
            Debug.LogWarning($"Индекс {ID_SegmentOnPlace} вне диапазона массива.");
        }
    }

}
