using System;
using Unity.Collections;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public int godIndex { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string resourceType { get; set; }


    public TileInfo(){}

    public TileInfo(int godIndex, int x, int y, string resourceType)
    {
        this.godIndex = godIndex;
        this.x = x;
        this.y = y;
        this.resourceType = resourceType;
    }
}
