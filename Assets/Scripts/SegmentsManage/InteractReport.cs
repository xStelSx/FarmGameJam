using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractReport : MonoBehaviour, IPointerClickHandler
{
    public int ID;

    private PlaceManager placeManager;

    void Start()
    {
        placeManager = FindObjectOfType<PlaceManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        Debug.Log($"ID поля = {ID}");
        if (placeManager != null)
        {
            placeManager.ReceiveMessage(ID);
        }
    }

}
