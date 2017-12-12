using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class DropOff : WorldObject {

    public int redOre, blueOre, greenOre = 0;
    public GameManager manager;

    public void ReceiveOre(Ore ore) {
        switch (ore.type) {
            case OreType.Red:
                redOre += 1;
                break;
            case OreType.Blue:
                blueOre += 1;
                break;
            case OreType.Green:
                greenOre += 1;
                break;
        }
        manager.UpdateScore();
    }
}
