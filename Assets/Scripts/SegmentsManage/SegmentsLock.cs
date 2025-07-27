using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentsLock : MonoBehaviour
{
    //public QuestManager questManager;
    public PlaceManager placeManager;
    public MarketInventory marketInventory;

    public int[] UpgradePlace;
    public int[] UpgradeMarket;
    public int[] AvailableSlots;

    void Start()
    {
        StartCoroutine(DelayedInitialActivation());
        //questManager = FindObjectOfType<QuestManager>();
        //placeManager = FindObjectOfType<PlaceManager>();
        //marketInventory = FindObjectOfType<MarketInventory>();
    }

    private IEnumerator DelayedInitialActivation()
    {
        // Ждем 1 секунду
        yield return new WaitForSeconds(1f);

        // Вызываем активацию слотов
        AktivateSlots(2);
    }

    //этот метод вызывается из квест менеджера при переходе на новый квест или из экспорта при продаже не так важно и передаётся сюда актуальный квест
    public void CheckCcurrentQuestIndex(int currentQuestIndex)
    {
        for (int i = 0; i < UpgradePlace.Length; i++)
        {
            if (currentQuestIndex == UpgradePlace[i])
            {
                if (currentQuestIndex == 0)
                {
                    UnLock_SegmentsOnPlace(6);
                    break;
                }
                if (currentQuestIndex == 1)
                {
                    UnLock_SegmentsOnPlace(18);
                    break;
                }
                if (currentQuestIndex == 2)
                {
                    UnLock_SegmentsOnPlace(36);
                    break;
                }

            }
        }

        for (int i = 0; i < UpgradeMarket.Length; i++)
        {
            if (currentQuestIndex == UpgradeMarket[i])
            {
                AktivateSlots(AvailableSlots[i]);
                break;
            }
        }
    }


    public void AktivateSlots(int UnlockSlots)
    {
        for (int i = 0; i < UnlockSlots; i++)
        {
            if (i < marketInventory.itemIcons.Count)
            {
                GameObject itemIcon = marketInventory.itemIcons[i];

                if (itemIcon != null)
                {
                    // Находим дочерний объект с именем "lock"
                    Transform lockChild = itemIcon.transform.Find("lock");

                    if (lockChild != null)
                    {
                        // Делаем что-то с объектом lock (например, деактивируем)
                        lockChild.gameObject.SetActive(false);
                        Debug.Log($"Слот {i} активирован. Объект 'lock' деактивирован.");
                    }
                    else
                    {
                        Debug.LogWarning($"Не найден дочерний объект 'lock' у иконки на индексе {i}.");
                    }

                    // Активируем саму иконку
                    itemIcon.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"Иконка на индексе {i} равна null.");
                }
            }
            else
            {
                Debug.LogWarning($"Индекс {i} выходит за пределы количества иконок.");
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