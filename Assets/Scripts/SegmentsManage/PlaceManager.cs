using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceManager : MonoBehaviour
{
    private MarketManager marketManager;

    public int ID_SegmentOnPlace;
    private int ID_SegmentOnMarket;

    public GameObject baseSegmentPrefab;
    public GameObject[] SegmentsPrefabPlace = new GameObject[36];
    public int?[] segmentArray = new int?[36];

    // Добавляем ссылки на системы частиц
    public ParticleSystem placeParticleSystem; // Эффект при размещении
    public ParticleSystem removeParticleSystem; // Эффект при удалении

    void Start()
    {
        marketManager = FindObjectOfType<MarketManager>();

        // Если системы частиц не назначены в инспекторе, попробуем найти их
        if (placeParticleSystem == null)
            placeParticleSystem = GameObject.Find("PlaceParticles")?.GetComponent<ParticleSystem>();
        if (removeParticleSystem == null)
            removeParticleSystem = GameObject.Find("RemoveParticles")?.GetComponent<ParticleSystem>();
    }

    public void ReceiveMessage(int id)
    {
        MarketReport();
        DetectActive_SegmentOnPlace(id);
    }

    public void MarketReport()
    {
        SoundManager.Instance.Play("ChoosePlaceSegment");
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
            // Воспроизводим эффект удаления перед удалением объекта
            PlayRemoveParticleEffect(SegmentsPrefabPlace[ID_SegmentOnPlace].transform.position);

            SetBasePrefab();
            segmentArray[ID_SegmentOnPlace] = null;
            SoundManager.Instance.Play("RemoveSegment");
            Debug.Log($"клетка {ID_SegmentOnPlace} стала свободной");
        }
        else
            Debug.Log($"клетка {ID_SegmentOnPlace} уже свободна");
    }

    public void Replacement()
    {
        if (segmentArray[ID_SegmentOnPlace] != null)
        {
            // Удаляем старый префаб, если он существует
            if (SegmentsPrefabPlace[ID_SegmentOnPlace] != null)
            {
                Destroy(SegmentsPrefabPlace[ID_SegmentOnPlace]);
            }
        }

        InterfaceSegments segment = FindObjectOfType<ISManager>().Segments[ID_SegmentOnMarket];
        GameObject farmPlace = GameObject.Find("FarmPlace");

        Vector3 spawnPosition = SegmentsPrefabPlace[ID_SegmentOnPlace].transform.position;
        Quaternion spawnRotation = Quaternion.Euler(0, SegmentsPrefabPlace[ID_SegmentOnPlace].transform.rotation.eulerAngles.y, 0);

        // Воспроизводим эффект размещения перед созданием объекта
        PlayPlaceParticleEffect(spawnPosition);

        GameObject newSegmentPrefab = Instantiate(
            segment.Prefab,
            spawnPosition,
            spawnRotation
        );

        if (farmPlace != null)
        {
            newSegmentPrefab.transform.SetParent(farmPlace.transform);
        }

        SegmentsPrefabPlace[ID_SegmentOnPlace] = newSegmentPrefab;
        segmentArray[ID_SegmentOnPlace] = ID_SegmentOnMarket;

        SegmentsPrefabPlace[ID_SegmentOnPlace].GetComponent<AddItems>().activeCollectionItem(ID_SegmentOnMarket);

        SoundManager.Instance.Play("PlaceSegment");
        Debug.Log($"На клетку {ID_SegmentOnPlace} установлен новый префаб {ID_SegmentOnMarket}");
    }

    public void SetBasePrefab()
    {
        if (SegmentsPrefabPlace[ID_SegmentOnPlace] != null)
        {
            // Удаляем старый префаб, если он существует
            Destroy(SegmentsPrefabPlace[ID_SegmentOnPlace]);
        }

        Vector3 spawnPosition = SegmentsPrefabPlace[ID_SegmentOnPlace].transform.position;
        GameObject farmPlace = GameObject.Find("FarmPlace");

        GameObject newBaseSegmentPrefab = Instantiate(baseSegmentPrefab, spawnPosition, Quaternion.identity);

        if (farmPlace != null)
        {
            newBaseSegmentPrefab.transform.SetParent(farmPlace.transform);
        }

        Transform firstChild = newBaseSegmentPrefab.transform.GetChild(0);
        InteractReport interactReport = firstChild.GetComponent<InteractReport>();
        interactReport.ID = ID_SegmentOnPlace;

        SegmentsPrefabPlace[ID_SegmentOnPlace] = newBaseSegmentPrefab;
        segmentArray[ID_SegmentOnPlace] = null;
        Debug.Log($"На клетку {ID_SegmentOnPlace} установлен базовый префаб");
    }

    // Методы для работы с частицами
    private void PlayPlaceParticleEffect(Vector3 position)
    {
        if (placeParticleSystem != null)
        {
            placeParticleSystem.transform.position = position;
            placeParticleSystem.Play();
        }
    }

    private void PlayRemoveParticleEffect(Vector3 position)
    {
        if (removeParticleSystem != null)
        {
            removeParticleSystem.transform.position = position;
            removeParticleSystem.Play();
        }
    }
}