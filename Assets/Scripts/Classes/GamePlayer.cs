using System;
using Unity.Collections;

public class GamePlayer
{
    public int godIndex { get; set; }
    public FixedString32Bytes godName { get; set; }
    public int militaryPoints { get; set; }
    public int culturePoints { get; set; }
    public int tradePoints { get; set; }
    public int techPoints { get; set; }
    public int productionPoints { get; set; }
    public int happiness { get; set; }

    public GamePlayer(){}

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
}
