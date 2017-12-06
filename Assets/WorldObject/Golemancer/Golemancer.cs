using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golemancer : Unit {
    
    protected override void PickUp(Ore ore) {
        if (carrying) {
            Drop();
        }

        Vector3 attachPos = transform.Find("StaffTop").position;
        ore.transform.position = attachPos;
        ore.transform.parent = transform;

        ore.OnPickUp(this);
        carrying = true;
    }

    public override void SetHoverState(WorldObject worldObject) {
        base.SetHoverState(worldObject);
        if(worldObject is Statue && carrying) {
            player.ui.SetCursorState(RTS.CursorState.PutDown);
        }
    }

}
