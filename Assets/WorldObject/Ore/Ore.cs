using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Ore : WorldObject {

    public OreType type;
    public Unit carrier { get; set; }

    public void OnPickUp(Unit pickerUper) {
        this.carrier = pickerUper;
    }

    public void OnDrop() {
        carrier = null;
    }
}
