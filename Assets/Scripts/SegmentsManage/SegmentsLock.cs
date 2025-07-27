using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentsLock : MonoBehaviour
{
    //public QuestManager questManager;
    private PlaceManager placeManager;

    public int[] UpgradeCondiion;

    void Start()
    {
        //questManager = FindObjectOfType<QuestManager>();
        placeManager = FindObjectOfType<PlaceManager>();
    }

    //этот метод вызывается из квест менеджера при переходе на новый квест или из экспорта при продаже не так важно и передаётся сюда актуальный квест
    public void CheckCcurrentQuestIndex(int currentQuestIndex)
    {
        for (int i = 0; i < UpgradeCondiion.Length; i++)
        {
            if (currentQuestIndex == UpgradeCondiion[i])
            {
                UnLock_SegmentsOnPlace((i + 1) * 6);
                break;
            }
        }
    }
    public void UnLock_SegmentsOnPlace(int UpgradeLVL)
    {
        // Убедитесь, что upgradeLevel не превышает размер массива
        int segmentsToUnlock = Mathf.Min(UpgradeLVL, placeManager.SegmentsPrefabPlace.Length);
        
        for (int i = 0; i < segmentsToUnlock; i++)
        {
            // Проверяем, что элемент в массиве не равен null
            if (placeManager.SegmentsPrefabPlace[i] != null)
            {
                GameObject segment = placeManager.SegmentsPrefabPlace[i]; // Получаем сегмент как GameObject
                
                // Проверяем, что сегмент не активен
                if (!segment.activeSelf) // Проверяем, активен ли сегмент
                {
                    segment.SetActive(true); // Разблокируем сегмент
                    Debug.Log($"Сегмент на индексе {i} разблокирован.");
                }
            }
            else
            {
                Debug.LogWarning($"Сегмент на индексе {i} равен null.");
            }
        }
    }

    
}
