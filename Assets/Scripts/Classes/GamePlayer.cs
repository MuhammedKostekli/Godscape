using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct GamePlayer : INetworkSerializable, IEquatable<GamePlayer>
{
    public int godIndex;
    public FixedString32Bytes godName;
    public int militaryPoints;
    public int culturePoints;
    public int tradePoints;
    public int techPoints;
    public int productionPoints;
    public int happiness;

    public GamePlayer(int godIndex, FixedString32Bytes godName, int militaryPoints, int culturePoints, int tradePoints, int techPoints, int productionPoints, int happiness)
    {
        this.godIndex = godIndex;
        this.godName = godName;
        this.militaryPoints = militaryPoints;
        this.culturePoints = culturePoints;
        this.tradePoints = tradePoints;
        this.techPoints = techPoints;
        this.productionPoints = productionPoints;
        this.happiness = happiness;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref godIndex);
        serializer.SerializeValue(ref godName);
        serializer.SerializeValue(ref militaryPoints);
        serializer.SerializeValue(ref culturePoints);
        serializer.SerializeValue(ref tradePoints);
        serializer.SerializeValue(ref techPoints);
        serializer.SerializeValue(ref productionPoints);
        serializer.SerializeValue(ref happiness);
    }


    public bool Equals(GamePlayer other)
    {
        return godIndex == other.godIndex;
    }

}


public class GamePlayerObj
{
    public int godIndex;
    public FixedString32Bytes godName;
    public int militaryPoints;
    public int culturePoints;
    public int tradePoints;
    public int techPoints;
    public int productionPoints;
    public int happiness;

    public GamePlayerObj(int godIndex, FixedString32Bytes godName, int militaryPoints, int culturePoints, int tradePoints, int techPoints, int productionPoints, int happiness)
    {
        this.godIndex = godIndex;
        this.godName = godName;
        this.militaryPoints = militaryPoints;
        this.culturePoints = culturePoints;
        this.tradePoints = tradePoints;
        this.techPoints = techPoints;
        this.productionPoints = productionPoints;
        this.happiness = happiness;
    }
}
