using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using System;

[CreateAssetMenu]
public class QuestDataBase : ScriptableObject
{
    public List<QuestData> questStructures;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}

[Serializable]

public class QuestData
{
    public int questId;
    public List<SegmentRequirement> segmentRequirements;
}

[Serializable]

public class SegmentRequirement
{
    public int segmentId;
    public int quantityResourse;
}


