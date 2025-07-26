using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InterfaceSegments
{
    public Sprite Icon;
    public int Price;
    public GameObject Prefab;
}

public class ISManager : MonoBehaviour
{
    public List<InterfaceSegments> Segments = new List<InterfaceSegments>();
}