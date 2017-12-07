using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Unit {

    protected override void PickUp(Ore ore) {
        if (carrying) {
            Drop();
        }

        Vector3 attachPos = transform.Find("RightHand").position;
        ore.transform.position = attachPos;
        ore.transform.parent = transform;

        ore.OnPickUp(this);
        carrying = true;
    }
}
